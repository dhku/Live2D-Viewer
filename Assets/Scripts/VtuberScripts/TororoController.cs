﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// for Live2D model
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Assets.Scripts.Network;

public class TororoController : MonoBehaviour, IPacketListener
{
    private CubismModel model;

    // threshold to activate changes in whole model's x/z parameter
    // instead of just changes in head
    public float abs_body_roll_threshold = 30;
    public float abs_body_yaw_threshold = 30;
    public float abs_body_roll_yaw_max = 60;

    public float ear_max_threshold = 0.38f;
    public float ear_min_threshold = 0.30f;

    public float iris_left_ceiling = 0.2f;
    public float iris_right_ceiling = 0.85f;
    public float iris_up_ceiling = 0.8f;
    public float iris_down_ceiling = 0.2f;

    public float mar_max_threshold = 1.0f;
    public float mar_min_threshold = 0.0f;

    public float mouth_dist_min = 60.0f;
    public float mouth_dist_max = 80.0f;

    private float t1;       // for breath
    private float roll = 0, pitch = 0, yaw = 0;
    private float ear_left = 0, ear_right = 0;
    private float x_ratio_left = 0, y_ratio_left = 0, x_ratio_right = 0, y_ratio_right = 0;
    private float mar = 0, mouth_dist = 0;

    private bool blush = false;

    // Start is called before the first frame update
    void Start()
    {
        model = this.FindCubismModel();

        abs_body_roll_threshold = Mathf.Abs(abs_body_roll_threshold);
        abs_body_yaw_threshold = Mathf.Abs(abs_body_yaw_threshold);
        abs_body_roll_yaw_max = Mathf.Abs(abs_body_roll_yaw_max);

        // Load saved JSON data at start
        // Commented the following two lines to make it error-free when following the YT Tutorial
        // A full, updated version which works with the UI system can be found after importing the unity packages to a Unity project
        // GameObject.FindWithTag("GameController").GetComponent<UISystem>().LoadData();
        // GameObject.FindWithTag("GameController").GetComponent<UISystem>().InitUI();
    }



    // Update is called once per frame
    void Update()
    {
        //print(string.Format("Roll: {0:F}; Pitch: {1:F}; Yaw: {2:F}", roll, pitch, yaw));

        // control the blush of the avatar
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (blush == false)
                blush = true;
            else
                blush = false;
        }
    }

    // Apply all changes of control variables here~
    // https://docs.live2d.com/cubism-sdk-tutorials/about-parameterupdating-of-model/?locale=en_us
    // Tip 1
    void LateUpdate()
    {
        // yaw
        var parameter = model.Parameters[3];
        parameter.Value = -Mathf.Clamp(yaw, -30, 30);

        // pitch
        parameter = model.Parameters[4];
        parameter.Value = Mathf.Clamp(pitch, -30, 30);

        // roll
        parameter = model.Parameters[5];
        parameter.Value = -Mathf.Clamp(roll, -30, 30);

        // breath
        t1 += Time.deltaTime;
        float value = (Mathf.Sin(t1 * 3f) + 1) * 0.5f;
        parameter = model.Parameters[21];
        parameter.Value = value;

        if (blush)
        {
            parameter = model.Parameters[3];
            parameter.Value = 1;
        }
        else
        {
            parameter = model.Parameters[3];
            parameter.Value = 0;
        }

        EyeBlinking();

        IrisMovement();

        MouthOpening();

    }

    // whole body movement (body X/Z)
    // optional as the effect is not that pronounced
    void BodyMovement()
    {
        // roll BODY Z
        var parameter = model.Parameters[17];
        if (Mathf.Abs(roll) > abs_body_roll_threshold)
        {
            parameter.Value = -(10 - 0) / (abs_body_roll_yaw_max - abs_body_roll_threshold) * ((Mathf.Abs(roll) - abs_body_roll_threshold) * Mathf.Sign(roll));
        }
        else
        {
            parameter.Value = 0;
        }

        // yaw BODY X
        //parameter = model.Parameters[16];
        //if (Mathf.Abs(yaw) > abs_body_yaw_threshold)
        //{
        //    parameter.Value = -(10 - 0) / (abs_body_roll_yaw_max - abs_body_yaw_threshold) * ((Mathf.Abs(yaw) - abs_body_yaw_threshold) * Mathf.Sign(yaw));
        //}
        //else
        //{
        //    parameter = model.Parameters[16];
        //    parameter.Value = 0;
        //}
    }

    void EyeBlinking()
    {
        // my left eye = live2d's right (mirroring) EYE R OPEN
        ear_left = Mathf.Clamp(ear_left, ear_min_threshold, ear_max_threshold);
        float eye_L_value = (ear_left - ear_min_threshold) / (ear_max_threshold - ear_min_threshold) * 1;
        var parameter = model.Parameters[7];
        parameter.Value = eye_L_value;

        // my right eye = live2d's left (mirroring) EYE L OPEN
        ear_right = Mathf.Clamp(ear_right, ear_min_threshold, ear_max_threshold);
        float eye_R_value = (ear_right - ear_min_threshold) / (ear_max_threshold - ear_min_threshold) * 1;
        parameter = model.Parameters[6];
        parameter.Value = eye_R_value;
    }

    void IrisMovement()
    {
        float eyeball_x = (x_ratio_left + x_ratio_right) / 2;
        float eyeball_y = (y_ratio_left + y_ratio_right) / 2;

        eyeball_x = Mathf.Clamp(eyeball_x, iris_left_ceiling, iris_right_ceiling);
        eyeball_y = Mathf.Clamp(eyeball_y, iris_down_ceiling, iris_up_ceiling);

        // range is [-1, 1]
        eyeball_x = (eyeball_x - iris_left_ceiling) / (iris_right_ceiling - iris_left_ceiling) * 2 - 1;
        eyeball_y = (eyeball_y - iris_down_ceiling) / (iris_up_ceiling - iris_down_ceiling) * 2 - 1;

        // optional
        // pass the value to an "activation function"
        // to create a smoother effect (when the iris is near center)
        eyeball_x = Mathf.Pow(eyeball_x, 3);
        eyeball_y = Mathf.Pow(eyeball_y, 3);

        var parameter = model.Parameters[8]; //eye ball X
        parameter.Value = eyeball_x;
        parameter = model.Parameters[9]; //eye ball Y
        parameter.Value = eyeball_y;
    }

    void MouthOpening()
    {
        // mouth aspect ratio -> mouth opening
        float mar_clamped = Mathf.Clamp(mar, mar_min_threshold, mar_max_threshold);
        mar_clamped = (mar_clamped - mar_min_threshold) / (mar_max_threshold - mar_min_threshold) * 1;
        var parameter = model.Parameters[12]; //mouth open
        parameter.Value = mar_clamped;
    }


    public void OnUpdate(MovementPacket packet)
    {
        this.roll = packet.roll;
        this.pitch = packet.pitch;
        this.yaw = packet.yaw;
        this.ear_left = packet.ear_left;
        this.ear_right = packet.ear_right;
        this.x_ratio_left = packet.x_ratio_left;
        this.y_ratio_left = packet.y_ratio_left;
        this.x_ratio_right = packet.x_ratio_right;
        this.y_ratio_right = packet.y_ratio_right;
        this.mar = packet.mar;
        this.mouth_dist = packet.mouth_dist;
    }
}

