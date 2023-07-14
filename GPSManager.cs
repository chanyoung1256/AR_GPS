using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GPSManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject gameObjectToInstantiate;
    public GameObject newObjectToInstantiate;
    public Text text;
    int count = 0;

    bool isCreated = false;
    location[] locations = new location[3];
    
     

    class location
    {
        public double latitude;
        public double longitude;
        public string name;

        public location(double lat, double lon, String n)
        {
            latitude = lat;
            longitude = lon;
            name = n;
        }
    }
    void Start()
    {
        //ARSessionOrigin m_SessionOrigin = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        //Transform m_Transform = m_SessionOrigin.gameObject.transform;
        //Vector3 pos = new Vector3(m_Transform.position.x, m_Transform.position.y - 0.5f, m_Transform.position.z + 1);
        //Quaternion e = Quaternion.Euler(0, 0, 0);
        //m_SessionOrigin.MakeContentAppearAt(Instantiate(gameObjectTolnstantiate).transform, pos, e);

        locations[0] = new location(37.886284, 127.735735, "한림대학교 성호관");
        locations[1] = new location(37.856813, 127.737247, "우리집");
        locations[2] = new location(37.866715, 127.738327, "한림대학교 정문");

        StartCoroutine(startGPS());

    }

    public double distance(double P1_latitude, double P1_longitude, double P2_latitude, double P2_longitude)
    {
        if ((P1_latitude == P2_latitude) && (P1_longitude == P2_longitude))
        {
            return 0;
        }
        double e10 = P1_latitude * Math.PI / 180;
        double e11 = P1_longitude * Math.PI / 180;
        double e12 = P2_latitude * Math.PI / 180;
        double e13 = P2_longitude * Math.PI / 180;
        /* 타원체 GRS80 */
        double c16 = 6356752.314140910;
        double c15 = 6378137.000000000;
        double c17 = 0.0033528107;
        double f15 = c17 + c17 * c17;
        double f16 = f15 / 2;
        double f17 = c17 * c17 / 2;
        double f18 = c17 * c17 / 8;
        double f19 = c17 * c17 / 16;
        double c18 = e13 - e11;
        double c20 = (1 - c17) * Math.Tan(e10);
        double c21 = Math.Atan(c20);
        double c22 = Math.Sin(c21);
        double c23 = Math.Cos(c21);
        double c24 = (1 - c17) * Math.Tan(e12);
        double c25 = Math.Atan(c24);
        double c26 = Math.Sin(c25);
        double c27 = Math.Cos(c25);
        double c29 = c18;
        double c31 = (c27 * Math.Sin(c29) * c27 * Math.Sin(c29))
          + (c23 * c26 - c22 * c27 * Math.Cos(c29))
          * (c23 * c26 - c22 * c27 * Math.Cos(c29));
        double c33 = (c22 * c26) + (c23 * c27 * Math.Cos(c29));
        double c35 = Math.Sqrt(c31) / c33;
        double c36 = Math.Atan(c35);
        double c38 = 0;
        if (c31 == 0)
        {
            c38 = 0;
        }
        else
        {
            c38 = c23 * c27 * Math.Sin(c29) / Math.Sqrt(c31);
        }
        double c40 = 0;
        if ((Math.Cos(Math.Asin(c38)) * Math.Cos(Math.Asin(c38))) == 0)
        {
            c40 = 0;
        }
        else
        {
            c40 = c33 - 2 * c22 * c26
              / (Math.Cos(Math.Asin(c38)) * Math.Cos(Math.Asin(c38)));
        }
        double c41 = Math.Cos(Math.Asin(c38)) * Math.Cos(Math.Asin(c38))
          * (c15 * c15 - c16 * c16) / (c16 * c16);
        double c43 = 1 + c41 / 16384
          * (4096 + c41 * (-768 + c41 * (320 - 175 * c41)));
        double c45 = c41 / 1024 * (256 + c41 * (-128 + c41 * (74 - 47 * c41)));
        double c47 = c45
          * Math.Sqrt(c31)
          * (c40 + c45
            / 4
            * (c33 * (-1 + 2 * c40 * c40) - c45 / 6 * c40
              * (-3 + 4 * c31) * (-3 + 4 * c40 * c40)));
        double c50 = c17
          / 16
          * Math.Cos(Math.Asin(c38))
          * Math.Cos(Math.Asin(c38))
          * (4 + c17
            * (4 - 3 * Math.Cos(Math.Asin(c38))
              * Math.Cos(Math.Asin(c38))));
        double c52 = c18
          + (1 - c50)
          * c17
          * c38
          * (Math.Acos(c33) + c50 * Math.Sin(Math.Acos(c33))
            * (c40 + c50 * c33 * (-1 + 2 * c40 * c40)));
        double c54 = c16 * c43 * (Math.Atan(c35) - c47);
        // return distance in meter
        return c54;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator startGPS()
    {
        Input.location.Start(1.0f, 0.1f);

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            text.text = "Timed out";
            print("Request Timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            text.text = "Unable to determine device location";
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            StartCoroutine(updateGPS());
        }
    }
    public short bearingP1toP2(double P1_latitude, double P1_longitude, double P2_latitude, double P2_longitude)
    {
        double Cur_Lat_radian = P1_latitude * (3.141592 / 180);
        double Cur_Lon_radian = P1_longitude * (3.141592 / 180);

        double Dest_Lat_radian = P2_latitude * (3.141592 / 180);
        double Dest_Lon_radian = P2_longitude * (3.141592 / 180);

        double radian_distance = 0;
        radian_distance = Math.Acos(Math.Sin(Cur_Lat_radian)
          * Math.Sin(Dest_Lat_radian) + Math.Cos(Cur_Lat_radian)
          * Math.Cos(Dest_Lat_radian)
          * Math.Cos(Cur_Lon_radian - Dest_Lon_radian));

        double radian_bearing = Math.Acos((Math.Sin(Dest_Lat_radian) - Math
          .Sin(Cur_Lat_radian)
          * Math.Cos(radian_distance))
          / (Math.Cos(Cur_Lat_radian) * Math.Sin(radian_distance)));

        double true_bearing = 0;
        if (Math.Sin(Dest_Lon_radian - Cur_Lon_radian) < 0)
        {
            true_bearing = radian_bearing * (180 / 3.141592);
            true_bearing = 360 - true_bearing;
        }
        else
        {
            true_bearing = radian_bearing * (180 / 3.141592);
        }
        return (short)true_bearing;
    }
    IEnumerator updateGPS()
    {
        float UPDATE_TIME = 0.5f;
        double detailed_num = 1.0;
        WaitForSeconds updateTime = new WaitForSeconds(UPDATE_TIME);

        while (true)
        {
            var lastDate = Input.location.lastData;
            double latitude = lastDate.latitude * detailed_num;
            double longitude = lastDate.longitude * detailed_num;

            int range = 50;
            string result;
            bool isFind = false;
            result = range + "미터 이내에 감지된 좌표가 없습니다.";

            int shortestDis = 999999;
            location shortesrLoc = null;

            for (int i = 0; i < locations.Length; i++)
            {
                double targetLatitude = locations[i].latitude;
                double targetLongitude = locations[i].longitude;
                int dis = Math.Abs((int)distance(latitude, longitude, targetLatitude, targetLongitude));

                if(dis < shortestDis)
                {
                    shortestDis = dis;
                    shortesrLoc = locations[i];
                }

                if (dis < range)
                {
                    isFind = true;
                    result = "여기는 어디?\n" + locations[i].name;
                    if (!isCreated)
                    {
                        isCreated = true;
                        ARSessionOrigin m_SessionOrigin = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
                        Transform m_Transform = m_SessionOrigin.gameObject.transform;
                        Vector3 pos = new Vector3(m_Transform.position.x, m_Transform.position.y - 0.5f, m_Transform.position.z + 1);
                        Quaternion e = Quaternion.Euler(0, 0, 0);

                        GameObject target = null;
                        if (locations[i].name.Equals("회사"))
                        {
                            target = Instantiate(newObjectToInstantiate);
                        }
                        else
                        {
                            target = Instantiate(gameObjectToInstantiate);
                        }
                        target.name = "target";
                        m_SessionOrigin.MakeContentAppearAt(target.transform, pos, e);
                    }
                    break;
                }
            }
            if(!isFind)
            {
                short resultDeg = bearingP1toP2(latitude, longitude, shortesrLoc.latitude, shortesrLoc.longitude);
                result += "\n가장 가까운 위치는?\n" + resultDeg + "각도로" + shortestDis + "M만큼 떨어진" + shortesrLoc.name;
            }
            if (!isFind && GameObject.Find("target")) Destroy(GameObject.Find("target"));

            text.text = result;
            yield return updateTime;
        }
    }

    void stopGPS()
    {
        Input.location.Stop();
        StopCoroutine(updateGPS());
    }


}
