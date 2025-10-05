using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Ramen.Data;

#if UNITY_EDITOR
namespace Ramen.Editor
{
    /// <summary>
    /// CSVファイルからEnemyListにデータをインポートするEditor拡張
    /// </summary>
    public class EnemyListImporter : EditorWindow
    {
        private EnemyList targetEnemyList;
        private TextAsset csvFile;
        private Vector2 scrollPosition;
        private bool showPreview = false;
        private List<Enemy> previewEnemies = new List<Enemy>();

        [MenuItem("Ramen/CSV Importer/Enemy List Importer")]
        public static void ShowWindow()
        {
            var window = GetWindow<EnemyListImporter>("敵リスト インポーター");
            window.minSize = new Vector2(500, 350);
        }

        private void OnGUI()
        {
            GUILayout.Label("CSVファイルからEnemyListにデータをインポート", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // EnemyListの設定
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("対象EnemyList:", GUILayout.Width(120));
            targetEnemyList = (EnemyList)EditorGUILayout.ObjectField(targetEnemyList, typeof(EnemyList), false);
            EditorGUILayout.EndHorizontal();

            // CSVファイルの設定
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("CSVファイル:", GUILayout.Width(120));
            csvFile = (TextAsset)EditorGUILayout.ObjectField(csvFile, typeof(TextAsset), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // プレビューボタン
            if (GUILayout.Button("プレビュー"))
            {
                if (csvFile != null)
                {
                    PreviewCSVData();
                }
                else
                {
                    EditorUtility.DisplayDialog("エラー", "CSVファイルを選択してください。", "OK");
                }
            }

            // プレビュー表示
            if (showPreview && previewEnemies.Count > 0)
            {
                EditorGUILayout.Space();
                GUILayout.Label($"プレビュー ({previewEnemies.Count}件):", EditorStyles.boldLabel);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(220));
                foreach (var enemy in previewEnemies)
                {
                    EditorGUILayout.BeginHorizontal("box");
                    GUILayout.Label($"ID: {enemy.EnemyID}", GUILayout.Width(60));
                    GUILayout.Label($"Name: {enemy.EnemyName}", GUILayout.Width(140));
                    GUILayout.Label($"HP: {enemy.HP}", GUILayout.Width(90));
                    GUILayout.Label($"ATK: {enemy.AttackPower}", GUILayout.Width(90));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.Space();

            // 取り込むボタン
            GUI.enabled = targetEnemyList != null && csvFile != null;
            if (GUILayout.Button("取り込む", GUILayout.Height(30)))
            {
                ImportCSVData();
            }
            GUI.enabled = true;

            EditorGUILayout.Space();

            // ヘルプ情報
            EditorGUILayout.HelpBox(
                "使用方法:\n" +
                "1. 対象のEnemyListを選択\n" +
                "2. CSVファイルを選択\n" +
                "3. プレビューで内容を確認\n" +
                "4. 『取り込む』ボタンでインポート\n\n" +
                "CSV形式: EnemyID,EnemyName,HP,AttackPower (タブ/カンマ対応)",
                MessageType.Info
            );
        }

        /// <summary>
        /// CSVデータをプレビュー表示
        /// </summary>
        private void PreviewCSVData()
        {
            previewEnemies.Clear();

            try
            {
                string[] lines = csvFile.text.Split('\n');

                // ヘッダー行をスキップ
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] values = ParseCSVLine(line);
                    if (values.Length >= 4)
                    {
                        var enemy = new Enemy
                        {
                            EnemyID = int.TryParse(values[0].Trim(), out int id) ? id : 0,
                            EnemyName = values[1].Trim(),
                            HP = int.TryParse(values[2].Trim(), out int hp) ? hp : 0,
                            AttackPower = int.TryParse(values[3].Trim(), out int atk) ? atk : 0
                        };
                        previewEnemies.Add(enemy);
                    }
                }

                showPreview = true;
                Debug.Log($"プレビュー完了: {previewEnemies.Count}件の敵データを読み込みました。");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"CSVプレビューエラー: {e.Message}");
                EditorUtility.DisplayDialog("エラー", $"CSVファイルの読み込みに失敗しました。\n{e.Message}", "OK");
            }
        }

        /// <summary>
        /// CSVデータをEnemyListにインポート
        /// </summary>
        private void ImportCSVData()
        {
            if (targetEnemyList == null)
            {
                EditorUtility.DisplayDialog("エラー", "対象のEnemyListを選択してください。", "OK");
                return;
            }

            if (csvFile == null)
            {
                EditorUtility.DisplayDialog("エラー", "CSVファイルを選択してください。", "OK");
                return;
            }

            try
            {
                // 既存データをクリア
                targetEnemyList.ClearAllEnemies();

                string[] lines = csvFile.text.Split('\n');
                int importedCount = 0;

                // ヘッダー行をスキップ
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] values = ParseCSVLine(line);
                    if (values.Length >= 4)
                    {
                        var enemy = new Enemy
                        {
                            EnemyID = int.TryParse(values[0].Trim(), out int id) ? id : 0,
                            EnemyName = values[1].Trim(),
                            HP = int.TryParse(values[2].Trim(), out int hp) ? hp : 0,
                            AttackPower = int.TryParse(values[3].Trim(), out int atk) ? atk : 0
                        };
                        targetEnemyList.AddEnemy(enemy);
                        importedCount++;
                    }
                }

                // 変更を保存
                EditorUtility.SetDirty(targetEnemyList);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"インポート完了: {importedCount}件の敵データをEnemyListに追加しました。");
                EditorUtility.DisplayDialog("完了", $"{importedCount}件の敵データをEnemyListにインポートしました。", "OK");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"CSVインポートエラー: {e.Message}");
                EditorUtility.DisplayDialog("エラー", $"CSVファイルのインポートに失敗しました。\n{e.Message}", "OK");
            }
        }

        /// <summary>
        /// CSV行をパース（カンマ区切り、タブ区切りに対応）
        /// </summary>
        private string[] ParseCSVLine(string line)
        {
            // タブ区切りの場合はタブで分割、そうでなければカンマで分割
            if (line.Contains("\t"))
            {
                return line.Split('\t');
            }
            else
            {
                return line.Split(',');
            }
        }
    }
}
#endif