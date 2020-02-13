using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SCR_basket;

namespace SCR_basket
{
    public class Class1 : CTeam
    {
        public List<CPlayerModel> lLocalPlayer;
        public int[] LocalPosition;

        public Class1(List<CPlayerModel> _lLocalPlayer)
        {
            lLocalPlayer = _lLocalPlayer;
            LocalPosition = new int[10];
            foreach(int i in LocalPosition)
            {
                LocalPosition[i]= 0;
            }
        }

        public void View()
        {
            Thread.Sleep(200);
            while (!bEnd)
            {
                while (bPointPause) { } //Dopuki jest wąłczona pauza, czeka
                while (bDrawPause) { } //Dopóki jest włączona pauza, czeka
                foreach (CPlayerModel LocalPlayer in lLocalPlayer)
                {
                    for (int i = -100; i < 100; i++)
                    {
                        if (i != LocalPlayer.iPosition)
                        {
                            Console.Write("_");
                        }
                        else
                        {
                            Console.Write(LocalPlayer.iPlayerId);
                        }
                    }
                }
                Thread.Sleep(1000);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
