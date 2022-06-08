using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;


namespace Soco.ShaderVariantsStripper
{
    public class ShaderVariantsStripperConditionHasKeywordCombination
        : ShaderVariantsStripperCondition
    {
        public List<string> keywords = new List<string>();

        private enum AccessLevel
        {
            Global,
            Local
        }

        private AccessLevel _accessLevel = AccessLevel.Global;
        private int _selectedKeywordIndex;
        private string _inputKeyword;
        
        public bool Completion(Shader shader, ShaderVariantsData data)
        {
            int combinationValue = 0;
            foreach (string keyword in keywords)
            {
                combinationValue += data.shaderKeywordSet.IsEnabled(new ShaderKeyword(keyword)) ||
                                    data.shaderKeywordSet.IsEnabled(new ShaderKeyword(shader, keyword))
                    ? 1
                    : 0;
            }

            return combinationValue == keywords.Count && keywords.Count != 0;
        }
        
        public bool EqualTo(ShaderVariantsStripperCondition other)
        {
            if (other.GetType() != typeof(ShaderVariantsStripperConditionHasKeywordCombination))
            {
                return false;
            }

            ShaderVariantsStripperConditionHasKeywordCombination otherCondition =
                other as ShaderVariantsStripperConditionHasKeywordCombination;

            if (this.keywords.Count != otherCondition.keywords.Count)
            {
                return false;
            }
            
            var set1 = new HashSet<string>(this.keywords);
            var set2 = new HashSet<string>(otherCondition.keywords);
            return set1.SetEquals(set2);
        }

#if UNITY_EDITOR
        public string Overview()
        {
            string s = "当包含Keyword<";


            for (int i = 0; i < keywords.Count; ++i)
            {
                s += keywords[i];
                if (i != keywords.Count - 1)
                    s += ", ";
            }

            s += ">";
            if (keywords.Count > 1)
            {
                
                s += $"(共{keywords.Count}个)组合";
            }
                

            s += "时";

            return s;
        }
        
        public void OnGUI(ShaderVariantsStripperConditionOnGUIContext context)
        {
            EditorGUILayout.BeginVertical();
            if (context.shader != null)
            {
                #region 选择添加
                EditorGUILayout.BeginHorizontal();
                float width = 
                    EditorGUIUtility.currentViewWidth * 0.33f;
                
                AccessLevel newAccessLevel = (AccessLevel)EditorGUILayout.Popup((int)_accessLevel, new string[] { "Global", "Local" }, GUILayout.Width(width));

                if (newAccessLevel != _accessLevel)
                {
                    _selectedKeywordIndex = 0;
                    _accessLevel = newAccessLevel;
                }

                if (_accessLevel == AccessLevel.Global && context.globalKeywords.Length > 0 ||
                    _accessLevel == AccessLevel.Local && context.localKeywords.Length > 0)
                {
                    _selectedKeywordIndex = EditorGUILayout.Popup(_selectedKeywordIndex,
                        _accessLevel == AccessLevel.Global ? context.globalKeywords : context.localKeywords, GUILayout.Width(width));
                
                    string selectedKeyword = _accessLevel == AccessLevel.Global ? context.globalKeywords[_selectedKeywordIndex] : context.localKeywords[_selectedKeywordIndex];

                    if (GUILayout.Button("添加", GUILayout.Width(width)) && !keywords.Contains(selectedKeyword))
                    {
                        keywords.Add(selectedKeyword);
                    }
                }
                EditorGUILayout.EndHorizontal();
                #endregion
                
                EditorGUILayout.Space(20);
            }
            
            #region 输入添加
            EditorGUILayout.BeginHorizontal();

            _inputKeyword = EditorGUILayout.TextField("输入Keyword", _inputKeyword, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.66f));
            if (GUILayout.Button("添加", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.33f)) && _inputKeyword != null)
            {
                string[] inputKeywords = _inputKeyword.Split(' ');

                foreach (var keyword in inputKeywords)
                {
                    if(keyword != "" && !keywords.Contains(keyword))
                        keywords.Add(keyword);
                }
            }
            
            EditorGUILayout.EndHorizontal();
            #endregion
            
            EditorGUILayout.Space(20);
            
            #region 显示/删除Keyword

            EditorGUILayout.LabelField("当前Keyword:" + (keywords.Count == 0 ? " 无" : ""));
            EditorGUILayout.BeginHorizontal();
            
            const float itemWidth = 160.0f;
            float accumulationWidth = 0;
            
            for (int i = 0; i < keywords.Count; ++i)
            {
                if (accumulationWidth + itemWidth > 
                    EditorGUIUtility.currentViewWidth)
                {
                    accumulationWidth = 0;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                if (GUILayout.Button(new GUIContent(keywords[i], keywords[i]), GUILayout.Width(itemWidth)))
                {
                    keywords.RemoveAt(i);
                    break;
                }

                accumulationWidth += itemWidth;
            }
            
            EditorGUILayout.EndHorizontal();
            #endregion
            EditorGUILayout.EndVertical();
        }

        public string GetName() => "包含Keyword或集合";
#endif
    }

}
