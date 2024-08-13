using Assets.Scripts;
using Assets.Scripts.Network;
using Assets.Scripts.UI;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using TMPro;
using Debug = UnityEngine.Debug;

//메인 클래스
public class VtuberCore : MonoBehaviour
{
    private GameObject selectedModel;
    private string selectedModelKey;
    private IPacketListener selectedModelPacketListener;

    private GameObject background;
    private GameObject connectIcon;

    private UI sidebar;
    private UI modelSelect;
    private UI colorPanel;
    private UI settingsPanel;
    private UI backgroundPanel;

    private Dictionary<string, GameObject> prefabMap;
    private Dictionary<string, Sprite> spriteMap;

    private Client client;
    private bool isConnectionChecked = false;
    
    private Process process;
    private bool isProcessAlive = false;

    private UnityMainThreadDispatcher pool;

    public GameObject defaultPrefab;
    public GameObject BackgroundButtonPrefab;

    void Start()
    {
        this.process = new Process();

        this.prefabMap = new Dictionary<string, GameObject>();
        this.spriteMap = new Dictionary<string, Sprite>();
        this.selectedModelPacketListener = null;

        GameObject obj;
        instance = this;

        //UI 초기화
        {
            obj = GameObject.Find("Canvas_SideBar");
            this.sidebar = new SideBarUI(obj);
            obj = GameObject.Find("ModelSelectPanel");
            this.modelSelect = new ModelSelectUI(obj);
            obj = GameObject.Find("ColorPanel");
            this.colorPanel = new ColorPanelUI(obj);
            obj = GameObject.Find("SettingsPanel");
            this.settingsPanel = new SettingsUI(obj);
            obj = GameObject.Find("BackgroundPanel");
            this.backgroundPanel = new BackgroundUI(obj);

            //연결 아이콘
            obj = GameObject.Find("ConnectIcon");
            this.connectIcon = obj;

            //배경 이미지
            obj = GameObject.Find("BackgroundImage");
            this.background = obj;
            this.background.SetActive(false);
        }

        //리소스 로드
        {
            //모델링
            prefabMap.Add("Hiyori", Resources.Load("Prefabs/hiyori") as GameObject);
            prefabMap.Add("Akari", Resources.Load("Prefabs/akari") as GameObject);
            prefabMap.Add("Nekura", Resources.Load("Prefabs/nekura") as GameObject);
            prefabMap.Add("Tororo", Resources.Load("Prefabs/tororo") as GameObject);
            prefabMap.Add("Wanko", Resources.Load("Prefabs/wanko") as GameObject);

            if (defaultPrefab != null)
            {
                selectedModel = Instantiate(defaultPrefab);
                selectedModelKey = char.ToUpper(defaultPrefab.name[0]) + defaultPrefab.name.Substring(1); //앞글자 대문자 처리
            }
            else
            {
                selectedModel = Instantiate(this.prefabMap["Hiyori"]);
                selectedModelKey = "Hiyori";
            }

            selectedModelPacketListener = selectedModel.GetComponent<IPacketListener>();

            //Debug.Log((selectedModelPacketListener as HiyoriController).name);

        }

        {
            string path = Application.dataPath + "/Background";
            DirectoryInfo di = new DirectoryInfo(path);

            if (!di.Exists)
                Directory.CreateDirectory(path);
        }

        //서버 클라이언트
        {
            this.client = new Client(this);
            this.client.Start();
        }

        {
            this.pool = UnityMainThreadDispatcher.Instance();
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SideBarUI ui = sidebar as SideBarUI;
            if (ui.IsLocked())
            {
                ui.SetVisible(true);
                ui.SetIsLocked(false);
            }   
        }

        if(this.client.IsConnected() && isConnectionChecked == false)
        {
            this.connectIcon.SetActive(false);
            this.isConnectionChecked = true;
        }
    }

    public void ConnectionReset()
    {
        this.pool.Enqueue(() =>
        {
            this.Close();

            SideBarUI ui = sidebar as SideBarUI;
            ui.GetConnectButton().SetActive(true);

            this.connectIcon.SetActive(true);
            this.isConnectionChecked = false;

            
            this.client.InitTCP();
        });
    }

    void OnApplicationQuit()
    {
        this.Close();
    }

    public void Close()
    {
        if (this.client != null) this.client.Close();
        if (this.process != null && this.isProcessAlive && !this.process.HasExited) this.process.Kill();
    }

    public void UpdatePacket(MovementPacket packet)
    {
        if (selectedModelPacketListener != null)
        {
            selectedModelPacketListener.OnUpdate(packet);
        }
    }

    public void ChangeModel(string name)
    {
        if(!selectedModelKey.Equals(name))
        {
            Destroy(this.selectedModel);
            this.selectedModel = Instantiate(this.prefabMap[name]);
            this.selectedModelKey = name;
            this.selectedModelPacketListener = selectedModel.GetComponent<IPacketListener>();

            GameObject obj = this.selectedModel;
            float x = obj.transform.position.x;
            float y = obj.transform.position.y;
            Vector3 scale = obj.transform.localScale;
            SettingsUI settingsUI = this.settingsPanel as SettingsUI;
            settingsUI.init(x, y, scale);
        }
    }

    public void ChangeBackGround(string name)
    {
        //Debug.Log("Background_" + num);
        this.background.SetActive(true);
        Image renderer = this.background.GetComponent<Image>();
        renderer.sprite = this.spriteMap[name];

        BackgroundUI ui = this.backgroundPanel as BackgroundUI;
        ui.SetIsBackground(true);
    }

    public void DisableBackGround()
    {
        BackgroundUI ui = this.backgroundPanel as BackgroundUI;
    
        if(ui.IsBackground())
        {
            ui.SetIsBackground(false);
            this.background.SetActive(false);
            Image renderer = this.background.GetComponent<Image>();
            renderer.sprite = null;
        }
    }

    public void LockUI()
    {
        this.sidebar.SetVisible(false);
        this.modelSelect.SetVisible(false);
        this.colorPanel.SetVisible(false);
        this.settingsPanel.SetVisible(false);
        this.backgroundPanel.SetVisible(false);
    }

    public void UnlockUI()
    {
        this.sidebar.SetVisible(true);
    }

    public GameObject GetSelectedModel()
    {
        return this.selectedModel;
    }

    public void SetelectedModel(GameObject selectedModel)
    {
        this.selectedModel = selectedModel;
    }

    public Dictionary<string, GameObject> GetPrefabMap()
    {
        return this.prefabMap;
    }

    public UI GetModelSelectUI()
    {
        return this.modelSelect;
    }

    public UI GetColorPanelUI()
    {
        return this.colorPanel;
    }

    public UI GetSettingsUI()
    {
        return this.settingsPanel;
    }

    public UI GetBackgroundUI()
    {
        return this.backgroundPanel;
    }

    public void ReloadBackground()
    {
        this.spriteMap.Clear();

        string path = Application.dataPath + "/Background";
        DirectoryInfo di = new DirectoryInfo(path);

        foreach (FileInfo file in di.GetFiles())
        {
            if (file.Extension.Equals(".meta")) continue;
            string name = Path.GetFileNameWithoutExtension(file.Name);

            byte[] bytes = System.IO.File.ReadAllBytes(path + "/" + file.Name);
            Texture2D tex = new Texture2D(2,2);
            tex.LoadImage(bytes);
            spriteMap.Add(name, TextureToSprite(tex));
            //Debug.Log("파일명 : " + name);
        }
    }

    public void DestroyBackgroundButton()
    {
        GameObject content = GameObject.Find("BackgroundContent");
        Transform contentPanel = content.transform;

        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
    }

    public void CreateBackgroundButton()
    {
        GameObject content = GameObject.Find("BackgroundContent");
        Transform contentPanel = content.transform;

        foreach (KeyValuePair<string, Sprite> entry in spriteMap)
        {
            GameObject obj = Instantiate(this.BackgroundButtonPrefab, contentPanel);
            obj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = entry.Key;
        }

        BackgroundUI ui = this.backgroundPanel as BackgroundUI;
        ui.init();
    }

    public void CheckConnection()
    {
        StartCoroutine(CheckConnectionAlive());
    }

    IEnumerator CheckConnectionAlive()
    {
        yield return new WaitForSeconds(15.0f);

        if (!this.client.IsConnected())
        {
            SideBarUI ui = sidebar as SideBarUI;
            ui.GetConnectButton().SetActive(true);
        }

    }

    public Process GetProcess()
    {
        return this.process;
    }

    public void SetProcessAlive(bool isProcessAlive)
    {
        this.isProcessAlive = isProcessAlive;
    }

    public void KillProcess()
    {
        if (this.process != null && this.isProcessAlive && !this.process.HasExited) this.process.Kill();
    }

    public static VtuberCore getInstance()
    {
        return VtuberCore.instance;
    }

    public static VtuberCore instance;
    public static Sprite TextureToSprite(Texture2D texture) => Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 50f, 0, SpriteMeshType.FullRect);

}
