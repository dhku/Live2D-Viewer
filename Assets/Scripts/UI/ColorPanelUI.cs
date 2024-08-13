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
    public class ColorPanelUI : UI
    {
        Camera camera;

        public ColorPanelUI(GameObject canvas) : base(canvas)
        {
            this.SetVisible(false);
        }

        public override void load()
        {
            this.camera = GameObject.Find("Main Camera").GetComponent<Camera>();

            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                GameObject obj = canvas.transform.GetChild(i).gameObject;
                Button btn = obj.GetComponent<Button>();

                IntWrapper wrapper = new IntWrapper();
                wrapper.Value = i;

                btn.onClick.AddListener(() =>
                {
                    this.onClickColorPanel(wrapper); 
                });
            }
        }

        public void onClickColorPanel(IntWrapper idx)
        {

            this.manager.DisableBackGround();

            switch (idx.Value)
            {
                case 0:
                    camera.backgroundColor = new Color(1f, 1f, 1f); break;
                case 1:
                    camera.backgroundColor = new Color(1f, 0f, 0f); break;
                case 2:
                    camera.backgroundColor = new Color(1f, 0.5f, 0f); break;
                case 3:
                    camera.backgroundColor = new Color(1f, 1f, 0f); break;
                case 4:
                    camera.backgroundColor = new Color(0f, 1f, 0f); break;
                case 5:
                    camera.backgroundColor = new Color(0f, 0f, 1f); break;
                case 6:
                    camera.backgroundColor = new Color(0f, 0f, 0.5f); break;
                case 7:
                    camera.backgroundColor = new Color(0.5f, 0f, 0.5f); break;
                case 8:
                    camera.backgroundColor = new Color(0f, 0f, 0f); break;
            } 
        }
    }

    public class IntWrapper
    {
        public int Value { get; set; }
    }
}
