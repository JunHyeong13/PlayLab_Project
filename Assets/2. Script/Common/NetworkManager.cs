﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;


public class NetworkManager : MonoBehaviour
{
    /*
    Network Manager    
    블루투스 통신 및 센서통신을 위한 스크립트
    */

    private SerialPort serial;

    [SerializeField]
    private string boudRate = "115200";

    [SerializeField]
    private int timeOut = 110;

    [SerializeField]
    private Text sensorText;

    public GameObject logging;
    public GameObject rocketControl;
    private float outtakeTime = 0f;

    //PC용 데이터 저장 리스트   
    public List<string[]> dataList = new List<string[]>();
    public struct inputSensorData
    {
        public List<string[]> dataList;
        public string savePath;
    }
    inputSensorData forSendingMessage;

    //Singleton
    private static NetworkManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }



    // Start is called before the first frame update
    void Start()
    {


        //WinOS-OrangeBoard
        //serial = new SerialPort("COM3", int.Parse(boudRate), Parity.None, 8, StopBits.One);

        //WinOS-Nano 33 BLE
        //serial = new SerialPort("COM5", int.Parse(boudRate), Parity.None, 8, StopBits.One);

        //MacOS-Nano 33 BLE
        serial = new SerialPort("/dev/tty.usbmodem141201", int.Parse(boudRate), Parity.None, 8, StopBits.One);

        //AndroidOS-Nano 33 BLE
        //serial = new SerialPort("COM5", int.Parse(boudRate), Parity.None, 8, StopBits.One);

        //Configuramos control de datos por DTR.
        // We configure data control by DTR.
        serial.DtrEnable = true;
        
        

    }

    public void SensorStart()
    {
        //rocketControl.SendMessage("InHaleStart");
        try
        {
            serial.Open();
            serial.ReadTimeout = timeOut;
        }
        catch(System.IO.IOException e)
        {
            Debug.Log(e);
            //MacOS 시리얼 포트 초기화
            //serial = new SerialPort("/dev/tty.usbmodem141201", int.Parse(boudRate), Parity.None, 8, StopBits.One);
        }
        serial.Open();

    }

    public void stopSensor()
    {
        serial.Close();
        forSendingMessage.savePath = Application.streamingAssetsPath;
        forSendingMessage.dataList = dataList;
        logging.SendMessage("WriteCsvFile", forSendingMessage);
    }

    void Update()
    {

        chkSerial();
    }

    //시리얼이 열려있는지 검사. 타임아웃 시간 안에 데이터를 받아오면 이용, 아니면 타임아웃 에러 로그
    void chkSerial()
    {
        
        if (serial.IsOpen)
        {
            try
            {

                //Debug.Log(serial.ReadLine());
                sensorText.text = serial.ReadLine();

                //호흡데이터저장테스트용
                dataList.Add(dataToArray(sensorText.text));
            }
            catch (System.TimeoutException e)
            {
                Debug.Log(e);
                throw;
            }
            GameObject.Find("ArduinoState").GetComponent<Text>().text = "연결됨";
        }
        else if (!serial.IsOpen)
        {
            GameObject.Find("ArduinoState").GetComponent<Text>().text = "연결안됨";
        }
    }

    string[] dataToArray(string data)
    {
        string[] dataArray = new string[2];
        dataArray[0] = System.DateTime.Now.ToString();
        dataArray[1] = data;

        return dataArray;
    }

    private void OnApplicationQuit()
    {
        //logging.SendMessage("WriteCsv", dataList);
    }
}