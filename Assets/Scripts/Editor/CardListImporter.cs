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
    /// CSVファイルからCardListにデータをインポートするEditor拡張
    /// </summary>
    public class CardListImporter : EditorWindow
    {
        private CardList targetCardList;
        private TextAsset csvFile;
        private Vector2 scrollPosition;
        private bool showPreview = false;
        private List<Card> previewCards = new List<Card>();

        [MenuItem("Ramen/CSV Importer/Card List Importer")]
        public static void ShowWindow()
        {
            var window = GetWindow<CardListImporter>("カードリスト インポーター");
            window.minSize = new Vector2(400, 300);
        }

        private void OnGUI()
        {
            GUILayout.Label("CSVファイルからCardListにデータをインポート", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // CardListの設定
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("対象CardList:", GUILayout.Width(100));
            targetCardList = (CardList)EditorGUILayout.ObjectField(targetCardList, typeof(CardList), false);
            EditorGUILayout.EndHorizontal();

            // CSVファイルの設定
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("CSVファイル:", GUILayout.Width(100));
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
            if (showPreview && previewCards.Count > 0)
            {
                EditorGUILayout.Space();
                GUILayout.Label($"プレビュー ({previewCards.Count}件):", EditorStyles.boldLabel);
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
                foreach (var card in previewCards)
                {
                    EditorGUILayout.BeginHorizontal("box");
                    GUILayout.Label($"ID: {card.CardID}", GUILayout.Width(60));
                    GUILayout.Label($"Type: {card.CardType}", GUILayout.Width(40));
                    GUILayout.Label($"Name: {card.Name}", GUILayout.Width(100));
                    GUILayout.Label($"Power: {card.Power}", GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.Space();

            // 取り込むボタン
            GUI.enabled = targetCardList != null && csvFile != null;
            if (GUILayout.Button("取り込む", GUILayout.Height(30)))
            {
                ImportCSVData();
            }
            GUI.enabled = true;

            EditorGUILayout.Space();

            // ヘルプ情報
            EditorGUILayout.HelpBox(
                "使用方法:\n" +
                "1. 対象のCardListを選択\n" +
                "2. CSVファイルを選択\n" +
                "3. プレビューで内容を確認\n" +
                "4. 「取り込む」ボタンを押してインポート\n\n" +
                "CSV形式: CardID,CardType,CardTypeName,Name,Power",
                MessageType.Info
            );
        }

        /// <summary>
        /// CSVデータをプレビュー表示
        /// </summary>
        private void PreviewCSVData()
        {
            previewCards.Clear();
            
            try
            {
                string[] lines = csvFile.text.Split('\n');
                
                // ヘッダー行をスキップ
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] values = ParseCSVLine(line);
                    if (values.Length >= 5)
                    {
                        var card = new Card
                        {
                            CardID = values[0].Trim(),
                            CardType = CardTypeExtensions.FromLetter(values[1].Trim()),
                            CardTypeName = values[2].Trim(),
                            Name = values[3].Trim(),
                            Power = int.TryParse(values[4].Trim(), out int power) ? power : 0
                        };
                        previewCards.Add(card);
                    }
                }
                
                showPreview = true;
                Debug.Log($"プレビュー完了: {previewCards.Count}件のカードを読み込みました。");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"CSVプレビューエラー: {e.Message}");
                EditorUtility.DisplayDialog("エラー", $"CSVファイルの読み込みに失敗しました。\n{e.Message}", "OK");
            }
        }

        /// <summary>
        /// CSVデータをCardListにインポート
        /// </summary>
        private void ImportCSVData()
        {
            if (targetCardList == null)
            {
                EditorUtility.DisplayDialog("エラー", "対象のCardListを選択してください。", "OK");
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
                targetCardList.ClearAllCards();
                
                string[] lines = csvFile.text.Split('\n');
                int importedCount = 0;
                
                // ヘッダー行をスキップ
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] values = ParseCSVLine(line);
                    if (values.Length >= 5)
                    {
                        var card = new Card
                        {
                            CardID = values[0].Trim(),
                            CardType = CardTypeExtensions.FromLetter(values[1].Trim()),
                            CardTypeName = values[2].Trim(),
                            Name = values[3].Trim(),
                            Power = int.TryParse(values[4].Trim(), out int power) ? power : 0
                        };
                        targetCardList.AddCard(card);
                        importedCount++;
                    }
                }

                // 変更を保存
                EditorUtility.SetDirty(targetCardList);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"インポート完了: {importedCount}件のカードをCardListに追加しました。");
                EditorUtility.DisplayDialog("完了", $"{importedCount}件のカードをCardListにインポートしました。", "OK");
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
#endif