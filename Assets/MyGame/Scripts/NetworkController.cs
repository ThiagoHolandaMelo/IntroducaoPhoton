using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System.Net;
using UnityEngine.Networking;
using System.Collections.Generic;

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
        //Optando por iniciar num lobby antes de entrar numa sala
        PhotonNetwork.JoinLobby();
    }

    public void BotaoCriarSala()
    {
        string roomTemp = roomNameInput.text;
        RoomOptions roomOptions = new RoomOptions() {MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom(roomTemp, roomOptions, TypedLobby.Default);
    }


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
        PhotonNetwork.JoinRandomRoom();
    }

    //Tratando a falha ao entrar numa sala randomica
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string nomeDeSalaTemporaria = "Room" + UnityEngine.Random.Range(1000, 10000) + UnityEngine.Random.Range(1000, 10000);
        PhotonNetwork.CreateRoom(nomeDeSalaTemporaria);
    }

    //Método responsável por tratar o usuário pós entrada numa sala
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        Debug.Log("Room Name: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Current player in room: " + PhotonNetwork.CurrentRoom.PlayerCount);
		
		loginGO.gameObject.SetActive(false);
        partidasGO.gameObject.SetActive(false);
		
		//Instantiate(myPlayer, myPlayer.transform.position, myPlayer.transform.rotation);
		PhotonNetwork.Instantiate(myPlayer.name, myPlayer.transform.position, myPlayer.transform.rotation, 0);

    }

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
