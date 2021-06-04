//using System.Collections.Generic;
//using System.Linq;
//using HarmonyLib;
//using Photon.Pun;
//using TMPro;

//namespace FFAMod
//{
//    [HarmonyPatch(typeof(DisplayMatchPlayerNames))]
//    class DisplayMatchPlayerNamesPatch
//    {
//        [HarmonyPatch("ShowNames")]
//        private static bool Prefix(TextMeshProUGUI ___local, TextMeshProUGUI ___other)
//        {
//			List<Photon.Realtime.Player> list = (from p in PhotonNetwork.CurrentRoom.Players select p.Value).ToList();
//			bool flag = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(NetworkConnectionHandler.TWITCH_ROOM_AUDIENCE_RATING_KEY);
//			___other.text = null;
//		___local.text=null;
//		Player plr = null;
//					for (int i = 0; i < PlayerManager.instance.players.Count; i++)
//			{
//					if (PlayerManager.instance.players[i].data.view.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
//					{
//						plr = PlayerManager.instance.players[i];
//						break;
//					}
//			}

//			for (int i = 0; i < list.Count; i++)
//			{
//				string str = flag ? (" (" + list[i].CustomProperties[NetworkConnectionHandler.TWITCH_PLAYER_SCORE_KEY].ToString() + ")") : string.Empty;
//				if (plr.playerID == i)
//				{
//					___local.text = list[i].NickName + str;
//				}
//				else
//				{
//					___other.text = ___other.text + list[i].NickName + str + "\n";
//				}
//			}
//			___other.text = ___other.text.Substring(0, ___other.text.Length - 1);
//			return false;
//		}
//    }
//}
