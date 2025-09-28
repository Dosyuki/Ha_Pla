using UnityEngine;
using UnityEngine.UI;

public class TimeSystem : Singleton<TimeSystem>
{
    //variable to store a light source
        [SerializeField] private Light sun;
    
        //variable to store the time of the day
        [SerializeField, Range(0, 24)] private float timeOfDay;
    
        //variable to store the speed of rotation
        [SerializeField] private float sunRotationSpeed;
    
        //variables to store the lighting presets
        [Header("LightingPreset")]
        [SerializeField] private Gradient skyColor;
    
        [SerializeField] private Gradient equatorColor;
        [SerializeField] private Gradient sunColor;
        //function to update Sun's rotation
        [SerializeField] private Image dayLightUI;
    
        private void Update()
        {
            timeOfDay += (24f / sunRotationSpeed) * Time.deltaTime;
            if (timeOfDay > 24)
                timeOfDay = 0;
            UpdateSunRotation();
            UpdateLighting();
        }
    
        private void OnValidate()
        {
            UpdateSunRotation();
            UpdateLighting();
        }
    
        private void UpdateSunRotation()
        {
            float sunRotation = Mathf.Lerp(-90, 270, timeOfDay / 24);
            sun.transform.rotation = Quaternion.Euler(sunRotation, sun.transform.rotation.y, sun.transform.rotation.z);
            
            float uiRotation = Mathf.Lerp(-90, 270, timeOfDay / 24f);
            dayLightUI.transform.rotation = Quaternion.Euler(0, 0, uiRotation);
        }
    
        //fuction to update the lighting
    
        private void UpdateLighting()
        {
            float timeFraction = timeOfDay / 24;
            RenderSettings.ambientEquatorColor = equatorColor.Evaluate(timeFraction);
            RenderSettings.ambientSkyColor = skyColor.Evaluate(timeFraction);
            sun.color = sunColor.Evaluate(timeFraction);
        }

        public float[] GetDynamicCondition()
        {
            float[] condition = new float[2];
            if ((timeOfDay >= 0 && timeOfDay < 6) || (timeOfDay >= 20 && timeOfDay < 24))
            {
                condition[0] = 1.1f; //Luck
                condition[1] = 0.9f; //Weight
                return condition;
            }

            condition[0] = 1f;
            condition[1] = 1f;
            return condition;
        }
}