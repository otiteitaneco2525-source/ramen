using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Ramen.Data;

namespace Ramen.Editor
{
    /// <summary>
    /// CSVファイルからSerifListにデータをインポートするEditor拡張
    /// </summary>
    public class SerifListImporter : EditorWindow
    {
        private SerifList targetSerifList;
        private TextAsset csvFile;
        private Vector2 scrollPosition;
        private bool showPreview = false;
        private List<Serif> previewSerifs = new List<Serif>();

        [MenuItem("Ramen/CSV Importer/Serif List Importer")]
        public static void ShowWindow()
        {
            var window = GetWindow<SerifListImporter>("セリフリスト インポーター");
            window.minSize = new Vector2(500, 400);
        }

        private void OnGUI()
        {
            GUILayout.Label("CSVファイルからSerifListにデータをインポート", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // SerifListの設定
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("対象SerifList:", GUILayout.Width(120));
            targetSerifList = (SerifList)EditorGUILayout.ObjectField(targetSerifList, typeof(SerifList), false);
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
            if (showPreview && previewSerifs.Count > 0)
            {
                EditorGUILayout.Space();
                GUILayout.Label($"プレビュー ({previewSerifs.Count}件):", EditorStyles.boldLabel);
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(250));
                foreach (var serif in previewSerifs)
                {
                    EditorGUILayout.BeginVertical("box");
                    
                    // セリフIDとセリフ名
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label($"ID: {serif.SerifID}", GUILayout.Width(60));
                    GUILayout.Label($"セリフ: {serif.SerifName}", GUILayout.Width(300));
                    EditorGUILayout.EndHorizontal();
                    
                    // 出現率
                    EditorGUILayout.BeginHorizontal();
                    Color originalColor = GUI.color;
                    GUI.color = Color.blue;
                    GUILayout.Label($"通常バトル: {serif.NormalBattleRate}%", GUILayout.Width(120));
                    GUI.color = Color.red;
                    GUILayout.Label($"ボスバトル: {serif.BossBattleRate}%", GUILayout.Width(120));
                    GUI.color = originalColor;
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(2);
                }
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.Space();

            // 取り込むボタン
            GUI.enabled = targetSerifList != null && csvFile != null;
            if (GUILayout.Button("取り込む", GUILayout.Height(30)))
            {
                ImportCSVData();
            }
            GUI.enabled = true;

            EditorGUILayout.Space();

            // ヘルプ情報
            EditorGUILayout.HelpBox(
                "使用方法:\n" +
                "1. 対象のSerifListを選択\n" +
                "2. CSVファイルを選択\n" +
                "3. プレビューで内容を確認\n" +
                "4. 「取り込む」ボタンを押してインポート\n\n" +
                "CSV形式: SerifID,SerifName,NormalBattleRate,BossBattleRate",
                MessageType.Info
            );
        }

        /// <summary>
        /// CSVデータをプレビュー表示
        /// </summary>
        private void PreviewCSVData()
        {
            previewSerifs.Clear();
            
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
                        var serif = new Serif
                        {
                            SerifID = values[0].Trim(),
                            SerifName = values[1].Trim(),
                            NormalBattleRate = int.TryParse(values[2].Trim(), out int normalRate) ? normalRate : 0,
                            BossBattleRate = int.TryParse(values[3].Trim(), out int bossRate) ? bossRate : 0
                        };
                        previewSerifs.Add(serif);
                    }
                }
                
                showPreview = true;
                Debug.Log($"プレビュー完了: {previewSerifs.Count}件のセリフを読み込みました。");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"CSVプレビューエラー: {e.Message}");
                EditorUtility.DisplayDialog("エラー", $"CSVファイルの読み込みに失敗しました。\n{e.Message}", "OK");
            }
        }

        /// <summary>
        /// CSVデータをSerifListにインポート
        /// </summary>
        private void ImportCSVData()
        {
            if (targetSerifList == null)
            {
                EditorUtility.DisplayDialog("エラー", "対象のSerifListを選択してください。", "OK");
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
                targetSerifList.ClearAllSerifs();
                
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
                        var serif = new Serif
                        {
                            SerifID = values[0].Trim(),
                            SerifName = values[1].Trim(),
                            NormalBattleRate = int.TryParse(values[2].Trim(), out int normalRate) ? normalRate : 0,
                            BossBattleRate = int.TryParse(values[3].Trim(), out int bossRate) ? bossRate : 0
                        };
                        targetSerifList.AddSerif(serif);
                        importedCount++;
                    }
                }

                // 変更を保存
                EditorUtility.SetDirty(targetSerifList);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"インポート完了: {importedCount}件のセリフをSerifListに追加しました。");
                EditorUtility.DisplayDialog("完了", $"{importedCount}件のセリフをSerifListにインポートしました。", "OK");
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
        /// <param name="line">CSV行</param>
        /// <returns>分割された値の配列</returns>
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
