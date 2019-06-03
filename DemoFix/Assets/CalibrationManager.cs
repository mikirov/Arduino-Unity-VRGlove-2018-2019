using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CalibrationManager : MonoBehaviour
{
    [SerializeField]    
    private TMP_Text timerText;

    [SerializeField]
    private GameObject openHandPanel;

    [SerializeField]
    private GameObject closedHandPanel;

    [SerializeField]
    private float maxTimer = 5;

    [SerializeField]
    private float acceptableRestingTrimmerValueRange = 10;

    [SerializeField]
    private bool hasErrorChecking = true;

    private BaseInputController inputController;
    private HandController handController;

    private List<int> lastTrimmerValues;

    private int[] lowerBounds;
    private int[] highBounds;
    private int fingerJointCount;

    private float timer;

    private Color panelColor;

    private void Awake()
    {
        panelColor = GetComponent<Image>().color;
    }

    void Start()
    {
        inputController = FindObjectOfType<BaseInputController>();
        handController = FindObjectOfType<HandController>();

        fingerJointCount = handController.GetFingerJointCount();

        lowerBounds = new int[fingerJointCount];
        highBounds = new int[fingerJointCount];

        openHandPanel.SetActive(false);
        closedHandPanel.SetActive(false);

        Hide();
    }
    

    public void Calibrate()
    {
        StartCoroutine(Calibration());
    }


    private IEnumerator Calibration()
    {
        Debug.Log("Activate calibration");

        while (true)
        {
            handController.enabled = false;
            openHandPanel.SetActive(true);
            Show();

            lastTrimmerValues = inputController.GetValues();

            yield return WaitUntilResting();

            for (int i = 0; i < fingerJointCount; i++)
            {
                if (handController.GetFingerAt(i).HasPositiveRotation())
                {
                    lowerBounds[i] = lastTrimmerValues[i];
                }
                else
                {
                    highBounds[i] = lastTrimmerValues[i];
                }
            }

            openHandPanel.SetActive(false);
            closedHandPanel.SetActive(true);

            yield return WaitUntilResting();

            for (int i = 0; i < fingerJointCount; i++)
            {
                if (handController.GetFingerAt(i).HasPositiveRotation())
                {
                    highBounds[i] = lastTrimmerValues[i];
                }
                else
                {
                    lowerBounds[i] = lastTrimmerValues[i];
                }
            }

            closedHandPanel.SetActive(false);

            if(hasErrorChecking)
            {
                bool hasFinishedCalibration = true;
                for (int i = 0; i < lastTrimmerValues.Count; i++)
                {
                    if (highBounds[i] <= lowerBounds[i])
                    {
                        Debug.LogError("Min val is higher than or equal to max. Restarting calibration!");
                        hasFinishedCalibration = false;
                        break;
                    }
                }

                if (hasFinishedCalibration)
                {
                    handController.enabled = true;
                    handController.SetBounds(lowerBounds, highBounds);

                    Hide();
                    break;
                }
            }
        }

        CalibrateRotation();
    }


    private void CalibrateRotation()
    {
        Quaternion mpuQuaternion = FindObjectOfType<BaseInputController>().GetMPUValues();
        Quaternion forward = Quaternion.LookRotation(Vector3.forward);
        handController.SetCalibrationRotation(Quaternion.Inverse(forward) * mpuQuaternion);
    }


    private IEnumerator WaitUntilResting()
    {
        timer = maxTimer;
        while (timer >= 0)
        {
            timerText.text = ((int)timer).ToString();

            List<int> trimmerValues = inputController.GetValues();
            for (int i = 0; i < trimmerValues.Count; i++)
            {
                int offset = Mathf.Abs(trimmerValues[i] - lastTrimmerValues[i]);
                if (offset > acceptableRestingTrimmerValueRange)
                {
                    timer = maxTimer;
                    timerText.text = ((int)timer).ToString();
                    break;
                }
            }
            lastTrimmerValues = new List<int>(trimmerValues);

            yield return new WaitForSeconds(1);
            timer -= 1;
        }
    }


    private void Hide()
    {
        Image panel = GetComponent<Image>();
        panel.raycastTarget = false;
        panel.color = new Color(0, 0, 0, 0);
        timerText.gameObject.SetActive(false);
    }


    private void Show()
    {
        Image panel = GetComponent<Image>();
        panel.raycastTarget = true;
        panel.color = panelColor;
        timerText.gameObject.SetActive(true);
    }
}
