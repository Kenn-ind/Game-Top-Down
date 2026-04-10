// Letakkan file ini di folder: Assets/Editor/
// Cara pakai: Unity Menu -> Tools -> Enemy Animator -> Add Death To All Animators

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

public class EnemyAnimatorDeathAdder : EditorWindow
{
    private string animatorFolderPath = "Assets";
    private bool previewOnly = true;
    private Vector2 scrollPos;
    private string log = "";

    [MenuItem("Tools/Enemy Animator/Add Death To All Animators")]
    public static void ShowWindow()
    {
        GetWindow<EnemyAnimatorDeathAdder>("Death Anim Adder");
    }

    void OnGUI()
    {
        GUILayout.Label("Batch Add Death Animation", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Folder picker
        EditorGUILayout.BeginHorizontal();
        animatorFolderPath = EditorGUILayout.TextField("Folder Animator", animatorFolderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(70)))
        {
            string selected = EditorUtility.OpenFolderPanel("Pilih folder Animator Controller", "Assets", "");
            if (!string.IsNullOrEmpty(selected))
            {
                // Konversi absolute path ke relative path
                if (selected.StartsWith(Application.dataPath))
                    animatorFolderPath = "Assets" + selected.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        previewOnly = EditorGUILayout.Toggle("Preview Only (Dry Run)", previewOnly);

        EditorGUILayout.HelpBox(
            previewOnly
                ? "Mode PREVIEW: tidak ada perubahan, hanya tampilkan apa yang akan diubah."
                : "Mode APPLY: akan langsung mengubah semua Animator Controller di folder tersebut.",
            previewOnly ? MessageType.Info : MessageType.Warning
        );

        EditorGUILayout.Space();

        if (GUILayout.Button(previewOnly ? "Preview" : "Apply Sekarang", GUILayout.Height(35)))
        {
            log = "";
            RunBatch(previewOnly);
        }

        EditorGUILayout.Space();
        GUILayout.Label("Log:", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
        EditorGUILayout.TextArea(log, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    void RunBatch(bool dryRun)
    {
        // Cari semua .controller di folder yang dipilih
        string[] guids = AssetDatabase.FindAssets("t:AnimatorController", new[] { animatorFolderPath });

        if (guids.Length == 0)
        {
            log += $"Tidak ada Animator Controller ditemukan di: {animatorFolderPath}\n";
            return;
        }

        log += $"Ditemukan {guids.Length} Animator Controller\n";
        log += dryRun ? "[PREVIEW MODE]\n\n" : "[APPLY MODE]\n\n";

        int modified = 0;
        int skipped = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AnimatorController ac = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

            if (ac == null) continue;

            log += $"--- {ac.name} ({path})\n";

            bool needsParam = !HasParameter(ac, "Die");
            bool needsState = !HasState(ac, "Death");

            if (!needsParam && !needsState)
            {
                log += "    ✓ Sudah lengkap, skip.\n\n";
                skipped++;
                continue;
            }

            if (dryRun)
            {
                if (needsParam) log += "    → Akan tambah parameter: Die (Trigger)\n";
                if (needsState) log += "    → Akan tambah state: Death\n";
                if (needsState) log += "    → Akan tambah transisi: AnyState → Death (Die)\n";
            }
            else
            {
                // Tambah parameter IsDead
                if (needsParam)
                {
                    ac.AddParameter(name: "Die", AnimatorControllerParameterType.Trigger);
                    log += "    + Parameter Die (Trigger) ditambahkan\n";
                }

                // Tambah state Death di layer 0
                AnimatorStateMachine sm = ac.layers[0].stateMachine;
                AnimatorState deathState = null;

                if (needsState)
                {
                    deathState = sm.AddState("Death");
                    // Posisi state di graph — taruh di bawah state lain
                    deathState.speed = 1f;
                    log += "    + State 'Death' ditambahkan\n";

                    // Tambah transisi AnyState → Death
                    AnimatorStateTransition transition = sm.AddAnyStateTransition(deathState);
                    transition.hasExitTime = false;
                    transition.duration = 0f;
                    transition.offset = 0f;
                    transition.canTransitionToSelf = false;

                    // Kondisi: IsDead = true
                    transition.AddCondition(AnimatorConditionMode.If, 0, "Die");
                    log += "    + Transisi AnyState → Death ditambahkan\n";

                    // Pindahkan transisi Death ke urutan PALING ATAS di AnyState
                    // agar priority-nya lebih tinggi dari Walk & Attack
                    ReorderDeathTransitionToTop(sm);
                    log += "    + Priority transisi Death dipindah ke urutan pertama\n";
                }

                EditorUtility.SetDirty(ac);
                modified++;
            }

            log += "\n";
        }

        if (!dryRun)
        {
            AssetDatabase.SaveAssets();
            log += $"\n✅ Selesai! {modified} Animator diubah, {skipped} di-skip (sudah lengkap).";
        }
        else
        {
            log += $"\n[Preview selesai] {guids.Length - skipped} Animator akan diubah, {skipped} akan di-skip.";
        }
    }

    // ─── Helper: cek apakah parameter sudah ada ───────────────────────────────
    bool HasParameter(AnimatorController ac, string paramName)
    {
        foreach (var p in ac.parameters)
            if (p.name == paramName) return true;
        return false;
    }

    // ─── Helper: cek apakah state sudah ada di layer 0 ───────────────────────
    bool HasState(AnimatorController ac, string stateName)
    {
        foreach (var s in ac.layers[0].stateMachine.states)
            if (s.state.name == stateName) return true;
        return false;
    }

    // ─── Helper: pindahkan transisi Death ke index 0 (priority tertinggi) ────
    void ReorderDeathTransitionToTop(AnimatorStateMachine sm)
    {
        var transitions = new System.Collections.Generic.List<AnimatorStateTransition>(sm.anyStateTransitions);

        int deathIndex = transitions.FindIndex(t =>
            t.destinationState != null && t.destinationState.name == "Death");

        if (deathIndex > 0)
        {
            var deathTransition = transitions[deathIndex];
            transitions.RemoveAt(deathIndex);
            transitions.Insert(0, deathTransition);
            sm.anyStateTransitions = transitions.ToArray();
        }
    }
}