using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BallBusterX
{
    class LightParameters
    {
        private float[] lightType = new float[MaxLights];
        private Vector3[] lightPosition = new Vector3[MaxLights];
        private Vector3[] lightColor = new Vector3[MaxLights];
        private Vector3[] lightAttenuation = new Vector3[MaxLights];
        private float[] lightSpecularExponent = new float[MaxLights];

        public const int MaxLights = 10;
        
        public void SetLightEnable(int index, bool enable)
        {
            if (index >= MaxLights)
                return;

            lightType[index] = enable ? 1 : 0;
        }

        public void Clear()
        {
            for (int i = 0; i < MaxLights; i++)
                SetLightEnable(i, false);
        }

        public void SetLightPosition(int index, Vector3 value)
        {
            if (index >= MaxLights)
                return;

            lightPosition[index] = value;
        }

        public void SetLightColor(int index, Color fireball)
        {
            if (index >= MaxLights)
                return;

            lightColor[index] = fireball.ToVector3();
        }

        public void SetAttenuation(int index, Vector3 attenuation)
        {
            if (index >= MaxLights)
                return;

            lightAttenuation[index] = attenuation;
        }

        public Color AmbientLightColor { get; set; }

        public void ApplyTo(Effect effect)
        {
            effect.Parameters["AmbientLightColor"].SetValue(AmbientLightColor.ToVector3());
            effect.Parameters["LightType"].SetValue(lightType);
            effect.Parameters["LightPosition"].SetValue(lightPosition);
            effect.Parameters["LightColor"].SetValue(lightColor);
            effect.Parameters["LightAttenuation"].SetValue(lightAttenuation);
        }
    }
}
