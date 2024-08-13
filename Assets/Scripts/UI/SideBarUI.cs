using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Threading;

namespace Assets.Scripts.UI
{
    public class SideBarUI : UI
    {
        private List<GameObject> buttons;
        Camera camera;

        private bool isLocked = false;

        public SideBarUI(GameObject canvas) : base(canvas)
        {
            this.SetVisible(true);
        }
    
        public override void load()
        {
            this.buttons = new List<GameObject>();

            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                GameObject obj = canvas.transform.GetChild(i).gameObject;
                this.buttons.Add(obj);
                Button btn = obj.GetComponent<Button>();

                switch (i)
                {
                    case 0:
                        btn.onClick.AddListener(OnConnectButton); break;
                    case 1:
                        btn.onClick.AddListener(OnClickModelButton); break;
                    case 2:
                        btn.onClick.AddListener(OnClickColorPanelButton); break;
                    case 3:
                        btn.onClick.AddListener(OnClickBackGroundButton); break;
                    case 4:
                        btn.onClick.AddListener(OnClickSettingsButton); break;
                    case 5:
                        btn.onClick.AddListener(OnClickLockButton); break;
                    case 6:
                        btn.onClick.AddListener(OnClickExitButton); break;
                }
            }

#if UNITY_STANDALONE_OSX
            buttons[0].SetActive(false);
#endif
            this.camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }

        public void OnConnectButton()
        {
            //Debug.Log("Clicked ConnectButton");
            buttons[0].SetActive(false);

            Thread t = new Thread(()=> {
                try
                {
                    Process process = this.manager.GetProcess();
                    process.StartInfo.FileName = Application.dataPath + "/start.bat";

                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false; // 프로세스를 시작할때 운영체제 셸을 사용할지

                    bool isProcessAlive = process.Start();
                    this.manager.SetProcessAlive(isProcessAlive); 
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError("Unable to launch app: " + e.Message);
                }
            });

            t.IsBackground = true;
            t.Start();

            this.manager.CheckConnection();
        }


        public void OnClickModelButton()
        {
            //Debug.Log("Clicked ModelButton");
            UI ui = this.manager.GetModelSelectUI();
            if(ui.IsVisible())
            {
                ui.SetVisible(false);
            }
            else
            {
                ui.SetVisible(true);
            }
        }

        public void OnClickColorPanelButton()
        {
            //Debug.Log("Clicked ColorPanelButton");
            UI ui = this.manager.GetColorPanelUI();
            if (ui.IsVisible())
            {
                ui.SetVisible(false);
            }
            else
            {
                ui.SetVisible(true);
            }
        }

        public void OnClickBackGroundButton()
        {
            //Debug.Log("Clicked BackGroundButton");
            UI ui = this.manager.GetBackgroundUI();
            if (ui.IsVisible())
            {
                this.manager.DestroyBackgroundButton();
                ui.SetVisible(false);
            }
            else
            {
                ui.SetVisible(true);
                this.manager.DestroyBackgroundButton();
                this.manager.ReloadBackground();
                this.manager.CreateBackgroundButton();
            }
        }

        public void OnClickSettingsButton()
        {
            //Debug.Log("Clicked SettingsButton");
            UI ui = this.manager.GetSettingsUI();
            if (ui.IsVisible())
            {
                ui.SetVisible(false);
            }
            else
            {
                GameObject obj = this.manager.GetSelectedModel();
                float x = obj.transform.position.x;
                float y = obj.transform.position.y;
                Vector3 scale = obj.transform.localScale;
                SettingsUI settingsUI = ui as SettingsUI;
                settingsUI.init(x, y, scale);
                settingsUI.SetVisible(true);
            }
        }

        //모든 UI를 숨깁니다.
        public void OnClickLockButton()
        {
            //Debug.Log("Clicked LockButton");
            this.manager.LockUI();
            this.isLocked = true;
        }

        public void OnClickExitButton()
        {
            //Debug.Log("Clicked ExitButton");
            this.manager.Close();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;       
#else
            Application.Quit();
#endif

        }

        public bool IsLocked()
        {
            return this.isLocked;
        }

        public void SetIsLocked(bool isLocked)
        {
            this.isLocked = isLocked;
        }

        public GameObject GetConnectButton()
        {
            return this.buttons[0];
        }

    }
}
