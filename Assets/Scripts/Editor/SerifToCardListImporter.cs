using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Ramen.Data;

namespace Ramen.Editor
{
    /// <summary>
    /// CSVファイルからSerifToCardListにデータをインポートするEditor拡張
    /// </summary>
    public class SerifToCardListImporter : EditorWindow
    {
        private SerifToCardList targetSerifToCardList;
        private TextAsset csvFile;
        private Vector2 scrollPosition;
        private bool showPreview = false;
        private List<SerifToCard> previewSerifToCards = new List<SerifToCard>();

        [MenuItem("Ramen/CSV Importer/Serif To Card List Importer")]
        public static void ShowWindow()
        {
            var window = GetWindow<SerifToCardListImporter>("セリフ→カードリスト インポーター");
            window.minSize = new Vector2(500, 400);
        }

        private void OnGUI()
        {
            GUILayout.Label("CSVファイルからSerifToCardListにデータをインポート", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // SerifToCardListの設定
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("対象SerifToCardList:", GUILayout.Width(140));
            targetSerifToCardList = (SerifToCardList)EditorGUILayout.ObjectField(targetSerifToCardList, typeof(SerifToCardList), false);
            EditorGUILayout.EndHorizontal();

            // CSVファイルの設定
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("CSVファイル:", GUILayout.Width(140));
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
            if (showPreview && previewSerifToCards.Count > 0)
            {
                EditorGUILayout.Space();
                GUILayout.Label($"プレビュー ({previewSerifToCards.Count}件):", EditorStyles.boldLabel);
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(250));
                foreach (var stc in previewSerifToCards)
                {
                    EditorGUILayout.BeginVertical("box");
                    
                    // セリフIDとカード情報
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label($"セリフID: {stc.SelfID}", GUILayout.Width(80));
                    GUILayout.Label("→", GUILayout.Width(20));
                    GUILayout.Label($"カード: {stc.CardName}({stc.CardID})", GUILayout.Width(200));
                    EditorGUILayout.EndHorizontal();
                    
                    // オプション
                    EditorGUILayout.BeginHorizontal();
                    Color originalColor = GUI.color;
                    if (stc.IsOtherThan())
                    {
                        GUI.color = Color.orange;
                        GUILayout.Label($"オプション: {stc.Option}", GUILayout.Width(150));
                    }
                    else
                    {
                        GUI.color = Color.green;
                        GUILayout.Label($"オプション: {stc.Option}", GUILayout.Width(150));
                    }
                    GUI.color = originalColor;
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(2);
                }
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.Space();

            // 取り込むボタン
            GUI.enabled = targetSerifToCardList != null && csvFile != null;
            if (GUILayout.Button("取り込む", GUILayout.Height(30)))
            {
                ImportCSVData();
            }
            GUI.enabled = true;

            EditorGUILayout.Space();

            // ヘルプ情報
            EditorGUILayout.HelpBox(
                "使用方法:\n" +
                "1. 対象のSerifToCardListを選択\n" +
                "2. CSVファイルを選択\n" +
                "3. プレビューで内容を確認\n" +
                "4. 「取り込む」ボタンを押してインポート\n\n" +
                "CSV形式: SelfID,CardID,CardName,Option",
                MessageType.Info
            );
        }

        /// <summary>
        /// CSVデータをプレビュー表示
        /// </summary>
        private void PreviewCSVData()
        {
            previewSerifToCards.Clear();
            
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
                        var stc = new SerifToCard
                        {
                            SelfID = values[0].Trim(),
                            CardID = values[1].Trim(),
                            CardName = values[2].Trim(),
                            Option = values[3].Trim()
                        };
                        previewSerifToCards.Add(stc);
                    }
                }
                
                showPreview = true;
                Debug.Log($"プレビュー完了: {previewSerifToCards.Count}件のセリフ→カード関連を読み込みました。");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"CSVプレビューエラー: {e.Message}");
                EditorUtility.DisplayDialog("エラー", $"CSVファイルの読み込みに失敗しました。\n{e.Message}", "OK");
            }
        }

        /// <summary>
        /// CSVデータをSerifToCardListにインポート
        /// </summary>
        private void ImportCSVData()
        {
            if (targetSerifToCardList == null)
            {
                EditorUtility.DisplayDialog("エラー", "対象のSerifToCardListを選択してください。", "OK");
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
                targetSerifToCardList.ClearAllSerifToCards();
                
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
                        var stc = new SerifToCard
                        {
                            SelfID = values[0].Trim(),
                            CardID = values[1].Trim(),
                            CardName = values[2].Trim(),
                            Option = values[3].Trim()
                        };
                        targetSerifToCardList.AddSerifToCard(stc);
                        importedCount++;
                    }
                }

                // 変更を保存
                EditorUtility.SetDirty(targetSerifToCardList);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"インポート完了: {importedCount}件のセリフ→カード関連をSerifToCardListに追加しました。");
                EditorUtility.DisplayDialog("完了", $"{importedCount}件のセリフ→カード関連をSerifToCardListにインポートしました。", "OK");
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
