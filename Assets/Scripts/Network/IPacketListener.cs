using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Network
{
    //해당 인터페이스는 각 모델 컨트롤러의 필드 값을 업데이트 합니다.
    public interface IPacketListener
    {
        void OnUpdate(MovementPacket packet);
    }
}
