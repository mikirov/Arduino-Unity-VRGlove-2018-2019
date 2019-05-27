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

    private BaseInputController inputController;
    private HandController handController;

    private List<int> lastTrimmerValues;

    private List<int> lowerBounds;
    private List<int> highBounds;

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

            CalibrateRotation();
            lowerBounds = new List<int>(lastTrimmerValues);

            openHandPanel.SetActive(false);
            closedHandPanel.SetActive(true);

            yield return WaitUntilResting();

            highBounds = new List<int>(lastTrimmerValues);

            closedHandPanel.SetActive(false);

            if (Application.isEditor)
            {
                handController.enabled = true;
                handController.SetBounds(lowerBounds.ToArray(), highBounds.ToArray());

                Hide();
                break;
            }
            else
            {
                bool hasFinishedCalibration = true;
                for (int i = 0; i < highBounds.Count; i++)
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
                    handController.SetBounds(lowerBounds.ToArray(), highBounds.ToArray());

                    Hide();
                    break;
                }
            }
        }
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
