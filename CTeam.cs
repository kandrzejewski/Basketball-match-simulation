using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SCR_basket;

namespace SCR_basket
{
    public class CTeam
    {
        public static bool bEnd; //Koniec meczu
        public static bool bPointPause; //Pauza po trafieniu
        public static bool bDrawPause = true; //Pauza na wylosowanie posiadacza piłki 
        public static int iScoreHost; //Wynik Gospodarzy
        public static int iScoreGuest; //Wynik Gości 
        public static bool bBallTeam; //Trużyna przy piłce
        public static bool bChanges = false; //Zmiany (podania, utraty)
        public static int iHaveBallID; //ID posiadacza piłki 
        public static int iAPBestEnergyLevel = -1; //Asystent z najwyższym poziomem energii
        public static int iCPBestEnergyLevel = -1; //Blokujący z najżyszym poziomem energii
        public static Mutex mMoveLock = new Mutex();
    }
}
