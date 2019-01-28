using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System.Net;
using UnityEngine.Networking;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkController : MonoBehaviourPunCallbacks {

    [Header("GO")]
    public GameObject loginGO;
    public GameObject partidasGO;

    [Header("Player")]
    public InputField playerNameInput;
    string playerNameTemp;
	public GameObject myPlayer;

    [Header("Room")]
    public InputField roomNameInput;

    Hashtable gameMode = new Hashtable();
    public byte gameMaxPlayer = 2;
    string gameModeKey = "gameMode";

    // Use this for initialization
    void Start () {
        playerNameTemp = "Player" + UnityEngine.Random.Range(1000, 10000);
        playerNameInput.text = playerNameTemp;

        roomNameInput.text = "Room" + UnityEngine.Random.Range(1000, 10000);

        loginGO.gameObject.SetActive(true);
        partidasGO.gameObject.SetActive(false);

    }

    public void Login()
    {
        //StartCoroutine(GetScores());
        //StartCoroutine(getUsuario("Thiago"));
        //StartCoroutine(addUsuario("Thiago", 30));
        
        if (playerNameInput.text != "")
        {
            PhotonNetwork.NickName = playerNameInput.text;
        }
        else
        {
            PhotonNetwork.NickName = playerNameTemp;
        }

        PhotonNetwork.ConnectUsingSettings();

        loginGO.gameObject.SetActive(false);        
    }

    public void BotaoBuscarPartidaRapida()
    {
        string[] typeGameRandom = new string[]{

            "PvP",
            "PvAI"
        };

        //gameMode.Add(gameModeKey, typeGameRandom[Random.Range(0, typeGameRandom.Length)]);

        //Optando por iniciar num lobby antes de entrar numa sala
        PhotonNetwork.JoinLobby();
    }

    public void BotaoCriarSala()
    {
        string roomTemp = roomNameInput.text;
        RoomOptions roomOptions = new RoomOptions() {MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom(roomTemp, roomOptions, TypedLobby.Default);
    }

    public void BotaoPartidaPvP()
    {
        gameMode.Add(gameModeKey, "PvP");
        PhotonNetwork.JoinLobby();
    }

    public void BotaoPartidaPvAI()
    {
        gameMode.Add(gameModeKey, "PvAI");
        PhotonNetwork.JoinLobby();
    }

    /*
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ListasDeSalas(roomList);
    }

    void ListasDeSalas(List<RoomInfo> roomList)
    {

        foreach (var item in roomList)
        {
            Debug.Log("Room Name existente: " + item.Name);
            
            Debug.Log("Room IsOpen: " + item.IsOpen);
            Debug.Log("Room IsVisible: " + item.IsVisible);
            Debug.Log("Room MaxPlayers: " + item.MaxPlayers);
            Debug.Log("Room PlayerCount: " + item.PlayerCount);

            object temp;
            item.CustomProperties.TryGetValue(gameModeKey, out temp);

            Debug.Log("Room CustomProperties: " + temp.ToString());
            
        }


    }
    */


    //######################## PunCallbacks ####################################
    public override void OnConnected()
    {
        Debug.Log("OnConnected");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        Debug.Log("Server: " + PhotonNetwork.CloudRegion + " ==> Ping: " + PhotonNetwork.GetPing());

        partidasGO.gameObject.SetActive(true);

        //Optando por iniciar num lobby antes de entrar numa sala
        //PhotonNetwork.JoinLobby();

    }

    //Ao entrar num Lobby, tentaremos entrar numa sala randomica
    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom(gameMode, 0);
        //PhotonNetwork.JoinRandomRoom();
    }

    //Tratando a falha ao entrar numa sala randomica
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");

        string roomTemp = "Room" + Random.Range(1000, 10000);

        RoomOptions options = new RoomOptions();
        options.IsOpen = true;
        options.IsVisible = true;
        options.MaxPlayers = gameMaxPlayer;
        options.CustomRoomProperties = gameMode;

        //atributo que permite que propriedades da sala, possam ser visualizadas fora dela 
        options.CustomRoomPropertiesForLobby = new string[] { gameModeKey };

        PhotonNetwork.CreateRoom(roomTemp, options);
    }

    //Método responsável por tratar o usuário pós entrada numa sala
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        Debug.Log("Room Name: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Current player in room: " + PhotonNetwork.CurrentRoom.PlayerCount);
		
		loginGO.gameObject.SetActive(false);
        partidasGO.gameObject.SetActive(false);

        object typeGameValue;

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(gameModeKey, out typeGameValue))
        {
            Debug.Log("GameMode: " + typeGameValue.ToString());
            Debug.Log("GameMode: " + (string)typeGameValue);
        }

        foreach (var item in PhotonNetwork.PlayerList)
        {
            //Debug.Log("Name: " + item.NickName);
            //Debug.Log("IsMaster? : " + item.IsMasterClient);

            Hashtable playerCustom = new Hashtable();
            playerCustom.Add("Lives", 3);
            playerCustom.Add("Score", 0);

            item.SetCustomProperties(playerCustom, null, null);

            item.SetScore(0);


        }

        //Instantiate(myPlayer, myPlayer.transform.position, myPlayer.transform.rotation);
        PhotonNetwork.Instantiate(myPlayer.name, myPlayer.transform.position, myPlayer.transform.rotation, 0);

    }

    //######################## WEBSERVICES PHP ####################################

    public void getUsuarios() {
        string sURL;
        string result = "";
        sURL = "https://alsharad.000webhostapp.com";

        HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(sURL);

        HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
        // Insert code that uses the response object.
        HttpWResp.Close();

        Debug.Log("result = " + result);
    }

    IEnumerator GetScores()
    {
        
        string sURL;
        sURL = "https://alsharad.000webhostapp.com";
        List<Score> listaScores = new List<Score>();

        UnityWebRequest request = UnityWebRequest.Get(sURL);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            string retornoRequest = request.downloadHandler.text;

            int quantidadeIteracoes = 0;

            while(retornoRequest.Length != 0 && quantidadeIteracoes < 6)
            {
                int startPosition = retornoRequest.IndexOf("{");

                if( startPosition < 0)
                {
                    break;
                }

                string elemento = retornoRequest.Substring(startPosition,
                                                        retornoRequest.IndexOf("}", startPosition) - startPosition + 1);

                Debug.Log("elemento => " + elemento);

                Score score = JsonUtility.FromJson<Score>(elemento);
                listaScores.Add( score );

                retornoRequest = retornoRequest.Replace(elemento, "");

                quantidadeIteracoes++;
            }

            foreach (Score scoreTemp in listaScores) {
                Debug.Log("chave => " + scoreTemp.chave);
                Debug.Log("nomeplayer => " + scoreTemp.nomeplayer);
                Debug.Log("pontuacao => " + scoreTemp.pontuacao);
            }
               
        }        

    }

    IEnumerator getUsuario( string nomeDoUsuario )
    {
        string sURL;
        sURL = "https://alsharad.000webhostapp.com/score/" + nomeDoUsuario;

        UnityWebRequest request = UnityWebRequest.Get(sURL);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            string retornoRequest = request.downloadHandler.text;

            retornoRequest = retornoRequest.Replace("[", "");
            retornoRequest = retornoRequest.Replace("]", "");

            Debug.Log("retornoRequest => " + retornoRequest);

            Score score = JsonUtility.FromJson<Score>(retornoRequest);

            Debug.Log("chave => " + score.chave);
            Debug.Log("nomeplayer => " + score.nomeplayer);
            Debug.Log("pontuacao => " + score.pontuacao);
            
        }        
    }

    IEnumerator addUsuario(string nomeplayer, int pontuacao)
    {
        string sURL;
        sURL = "https://alsharad.000webhostapp.com/score/add/" + nomeplayer + "/" + pontuacao;

        UnityWebRequest request = UnityWebRequest.Get(sURL);
        yield return request.SendWebRequest();

    
    }

}
