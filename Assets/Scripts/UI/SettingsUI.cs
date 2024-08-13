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
    class SettingsUI : UI
    {
        private List<Slider> sliderList; 

        public SettingsUI(GameObject canvas) : base(canvas)
        {
            this.SetVisible(false);
        }

        public override void load()
        {
            this.sliderList = new List<Slider>();

            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                GameObject obj = canvas.transform.GetChild(i).gameObject;

                Slider slider = obj.GetComponent<Slider>();

                if (slider == null) continue;
                else
                {
                    sliderList.Add(slider);

                    IntWrapper wrapper = new IntWrapper();
                    wrapper.Value = sliderList.Count - 1;

                    slider.onValueChanged.AddListener((value)=> 
                    {
                        this.onValueChanged(wrapper, value);
                    });
                }
                    
            }
        }

        public void onValueChanged(IntWrapper wrapper, float value)
        {
            int idx = wrapper.Value;
            GameObject obj = this.manager.GetSelectedModel();

            switch (idx)
            {
                case 0: // Pos X
                    obj.transform.position = new Vector3(value, obj.transform.position.y, obj.transform.position.z); break;
                case 1: // Pos Y
                    obj.transform.position = new Vector3(obj.transform.position.x, value, obj.transform.position.z); break;
                case 2: // Pos SCALE
                    obj.transform.localScale = new Vector3(value, value, value); break;
            }
        }

        public void init(float x, float y, Vector3 scale)
        {
            sliderList[0].value = x;
            sliderList[1].value = y;
            sliderList[2].value = scale.x;
        }

    }
}