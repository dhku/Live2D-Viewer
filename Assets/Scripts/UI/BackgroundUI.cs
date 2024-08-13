using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace Assets.Scripts.UI
{
    public class BackgroundUI : UI
    {
        private bool isBackground = false;

        public BackgroundUI(GameObject canvas) : base(canvas)
        {
            this.SetVisible(false);
        }

        public override void load(){}

        public void init()
        {
            GameObject content = canvas.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

            for (int i = 0; i < content.transform.childCount; i++)
            {
                GameObject obj = content.transform.GetChild(i).gameObject;
                Button btn = obj.GetComponent<Button>();

                String name = obj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text;

                btn.onClick.AddListener(() =>
                {
                    this.OnClickBackGroundButton(name);
                });
            }
        }

        public void OnClickBackGroundButton(string name)
        {
            this.isBackground = true;
            this.manager.ChangeBackGround(name);
        }

        public void SetIsBackground(bool isBackground)
        {
            this.isBackground = isBackground;
        }

        public bool IsBackground()
        {
            return this.isBackground;
        }
    }
}



