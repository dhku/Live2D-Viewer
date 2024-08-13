using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.Scripts.UI
{
    public class ModelSelectUI : UI
    {

        public ModelSelectUI(GameObject canvas) : base(canvas)
        {
            this.SetVisible(false);
        }

        public override void load()
        {
            GameObject buttonGroup = canvas.transform.GetChild(0).gameObject;

            for (int i = 0; i < buttonGroup.transform.childCount; i++)
            {
                GameObject obj = buttonGroup.transform.GetChild(i).gameObject;
                Button btn = obj.GetComponent<Button>();
                String name = obj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text;

                btn.onClick.AddListener(() => 
                {
                    this.manager.ChangeModel(name);
                });
            }

        }

    }
}
