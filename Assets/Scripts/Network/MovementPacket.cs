using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network
{
    //업데이트 대상 패러미터
    public class MovementPacket
    {
        public float t1;       // for breath
        public float roll = 0, pitch = 0, yaw = 0;
        public float ear_left = 0, ear_right = 0;
        public float x_ratio_left = 0, y_ratio_left = 0, x_ratio_right = 0, y_ratio_right = 0;
        public float mar = 0, mouth_dist = 0;
    }
}
