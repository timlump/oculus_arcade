﻿using UnityEngine;
using System.IO;

public class GameController : MonoBehaviour {

    public MeshRenderer displaySurface = null;
    public AudioSource audioSource = null;

    private LibRetroWrapper.RetroWrapper wrapper = null;
    private bool romLoaded = false;

    private string cueName = "TimeCrisis.cue";

    void CopyAssetsToPersistentData()
    {
        string path = "jar:file://" + Application.dataPath + "!/assets/";
        string cueFilePath = Application.persistentDataPath + "/TimeCrisis.cue";
        if (File.Exists(cueFilePath) == false)
        {
            WWW cue = new WWW(path + "TimeCrisis.cue");
            while (cue.isDone == false) { };
            File.WriteAllBytes(cueFilePath, cue.bytes);
        }

        string binFilePath = Application.persistentDataPath + "/TimeCrisis.bin";
        if (File.Exists(binFilePath) == false)
        {
            WWW bin = new WWW(path + "TimeCrisis.bin");
            while (bin.isDone == false) { };
            File.WriteAllBytes(binFilePath, bin.bytes);
        }

        string biosFilePath = Application.persistentDataPath + "/SCPH5502.BIN";
        if (File.Exists(biosFilePath) == false)
        {
            WWW bios = new WWW(path + "SCPH5502.BIN");
            while (bios.isDone == false) { };
            File.WriteAllBytes(biosFilePath, bios.bytes);
        }
    }

    // Use this for initialization
    void Start () {
        Application.targetFrameRate = 60;

        unsafe
        {

#if UNITY_EDITOR
            string systemPath = "D:/roms";
            string romPath = systemPath + "/" + cueName;

#else
            CopyAssetsToPersistentData();
            string systemPath = Application.persistentDataPath;
            string romPath = Application.persistentDataPath + "/" + cueName;
#endif

            wrapper = new LibRetroWrapper.RetroWrapper();
            wrapper.systemDirectory = systemPath;

            wrapper._audio = audioSource;
            wrapper.Initialise();

            romLoaded = wrapper.LoadRom(romPath);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (wrapper != null)
        {
            ControlUpdate();

            if (romLoaded)
            {
                wrapper.Update();

                if (wrapper.tex != null && displaySurface != null)
                {
                    displaySurface.sharedMaterial.mainTexture = wrapper.tex;
                }
            }
        }
	}

    private void OnDestroy()
    {
        if (wrapper != null)
        {
            Debug.Log("Destroying core");
            wrapper.Shutdown();
        }
    }

    private void ControlUpdate()
    {
        short delta = 400;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            wrapper.gun_x-=delta;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            wrapper.gun_x+=delta;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            wrapper.gun_y -= delta;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            wrapper.gun_y += delta;
        }

        wrapper.trigger = Input.GetKey(KeyCode.Space);
        wrapper.a_button = Input.GetKey(KeyCode.A);
        wrapper.b_button = Input.GetKey(KeyCode.D);
    }
}
