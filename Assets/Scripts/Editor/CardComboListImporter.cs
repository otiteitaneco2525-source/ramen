using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Ramen.Data;

namespace Ramen.Editor
{
    /// <summary>
    /// CSVファイルからCardComboListにデータをインポートするEditor拡張
    /// </summary>
    public class CardComboListImporter : EditorWindow
    {
        private CardComboList targetCardComboList;
        private TextAsset csvFile;
        private Vector2 scrollPosition;
        private bool showPreview = false;
        private List<CardCombo> previewCardCombos = new List<CardCombo>();

        [MenuItem("Ramen/CSV Importer/Card Combo List Importer")]
        public static void ShowWindow()
        {
            var window = GetWindow<CardComboListImporter>("カードコンボリスト インポーター");
            window.minSize = new Vector2(500, 400);
        }

        private void OnGUI()
        {
            GUILayout.Label("CSVファイルからCardComboListにデータをインポート", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // CardComboListの設定
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("対象CardComboList:", GUILayout.Width(120));
            targetCardComboList = (CardComboList)EditorGUILayout.ObjectField(targetCardComboList, typeof(CardComboList), false);
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
            if (showPreview && previewCardCombos.Count > 0)
            {
                EditorGUILayout.Space();
                GUILayout.Label($"プレビュー ({previewCardCombos.Count}件):", EditorStyles.boldLabel);
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(250));
                foreach (var combo in previewCardCombos)
                {
                    EditorGUILayout.BeginVertical("box");
                    
                    // 組み合わせ情報
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label($"{combo.CardID_From_Name}({combo.CardID_From})", GUILayout.Width(120));
                    GUILayout.Label("+", GUILayout.Width(20));
                    GUILayout.Label($"{combo.CardID_To_Name}({combo.CardID_To})", GUILayout.Width(120));
                    EditorGUILayout.EndHorizontal();
                    
                    // ボーナスとオプション
                    EditorGUILayout.BeginHorizontal();
                    string bonusText = combo.Bonus >= 0 ? $"+{combo.Bonus}" : combo.Bonus.ToString();
                    Color originalColor = GUI.color;
                    GUI.color = combo.IsPositiveBonus() ? Color.green : Color.red;
                    GUILayout.Label($"Bonus: {bonusText}", GUILayout.Width(100));
                    GUI.color = originalColor;
                    GUILayout.Label($"Option: {combo.Option}", GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(2);
                }
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.Space();

            // 取り込むボタン
            GUI.enabled = targetCardComboList != null && csvFile != null;
            if (GUILayout.Button("取り込む", GUILayout.Height(30)))
            {
                ImportCSVData();
            }
            GUI.enabled = true;

            EditorGUILayout.Space();

            // ヘルプ情報
            EditorGUILayout.HelpBox(
                "使用方法:\n" +
                "1. 対象のCardComboListを選択\n" +
                "2. CSVファイルを選択\n" +
                "3. プレビューで内容を確認\n" +
                "4. 「取り込む」ボタンを押してインポート\n\n" +
                "CSV形式: CardID_From,CardID_From_Name,CardID_To,CardID_To_Name,Bonus,Option",
                MessageType.Info
            );
        }

        /// <summary>
        /// CSVデータをプレビュー表示
        /// </summary>
        private void PreviewCSVData()
        {
            previewCardCombos.Clear();
            
            try
            {
                string[] lines = csvFile.text.Split('\n');
                
                // ヘッダー行をスキップ
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] values = ParseCSVLine(line);
                    if (values.Length >= 6)
                    {
                        var combo = new CardCombo
                        {
                            CardID_From = values[0].Trim(),
                            CardID_From_Name = values[1].Trim(),
                            CardID_To = values[2].Trim(),
                            CardID_To_Name = values[3].Trim(),
                            Bonus = int.TryParse(values[4].Trim(), out int bonus) ? bonus : 0,
                            Option = values[5].Trim()
                        };
                        previewCardCombos.Add(combo);
                    }
                }
                
                showPreview = true;
                Debug.Log($"プレビュー完了: {previewCardCombos.Count}件のカードコンボを読み込みました。");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"CSVプレビューエラー: {e.Message}");
                EditorUtility.DisplayDialog("エラー", $"CSVファイルの読み込みに失敗しました。\n{e.Message}", "OK");
            }
        }

        /// <summary>
        /// CSVデータをCardComboListにインポート
        /// </summary>
        private void ImportCSVData()
        {
            if (targetCardComboList == null)
            {
                EditorUtility.DisplayDialog("エラー", "対象のCardComboListを選択してください。", "OK");
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
                targetCardComboList.ClearAllCardCombos();
                
                string[] lines = csvFile.text.Split('\n');
                int importedCount = 0;
                
                // ヘッダー行をスキップ
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] values = ParseCSVLine(line);
                    if (values.Length >= 6)
                    {
                        var combo = new CardCombo
                        {
                            CardID_From = values[0].Trim(),
                            CardID_From_Name = values[1].Trim(),
                            CardID_To = values[2].Trim(),
                            CardID_To_Name = values[3].Trim(),
                            Bonus = int.TryParse(values[4].Trim(), out int bonus) ? bonus : 0,
                            Option = values[5].Trim()
                        };
                        targetCardComboList.AddCardCombo(combo);
                        importedCount++;
                    }
                }

                // 変更を保存
                EditorUtility.SetDirty(targetCardComboList);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"インポート完了: {importedCount}件のカードコンボをCardComboListに追加しました。");
                EditorUtility.DisplayDialog("完了", $"{importedCount}件のカードコンボをCardComboListにインポートしました。", "OK");
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
