using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInteract : MonoBehaviour {
    private RaycastHit hit;
    private int layerMask = 1 << 8, change, keys, doors, timer;
    [SerializeField]
    GameObject[] Reticles, switches, audioplayers;
    [SerializeField]
    Text[] texts;
    private List<string> messages, inventory;
    private string colstart = "<color=black>", colend = "</color>";
    private bool coderevealed = false, washedhands = false, gonetotoilet = false, wincol = false, gameoverplaying = false;
    private bool[] p;
    private Quest[] todo;
    [SerializeField]
    GameObject gameover, win;
    [SerializeField]
    // Use this for initialization
    void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        p = new bool[] {false, false, false, false, false };
        Time.timeScale = 0;
        timer = 180;
        StartCoroutine(Timer());
        layerMask = ~layerMask; //inverse layer mask for all layers but 8, so the raycast ignores the player
        foreach (Text t in texts)
        {
            t.text = string.Empty;
        }
        messages = new List<string>();
        inventory = new List<string>();
        todo = new Quest[] 
        {
            new Quest(">Escape.", false, true),//0
            new Quest(">Get to the toilet at the other end of the chambers before you po(o)p.", false, true),//1
            new Quest(">Find toilet paper.", false, true),//2
            new Quest(">Open Doors 0/4.", false, true),//3
            new Quest(">Activate switches.", false, true),//4
            new Quest(">Find Keys 0/2.", false, true),//5
            new Quest(">Find switch combination.", false, true),//6
            new Quest(">Admire Artwork 0/5", false, false),//7
            new Quest(">Find Change 0p/17p", false, false),//8
            new Quest(">Collect Rock, Paper & Scissors", false, false)//9
        };
        RedoTodo();
        texts[4].text = "Instructions; " + "\n" + "W, A, S, D to move," + "\n" + "E to open inventory," + "\n" + "Left Click to interact." + "\n" + "(Got it ? Left Click to start.)";
    }

    // Update is called once per frame
    void Update ()
    {
        if (Time.timeScale != 0 && timer > 0)
        {
            if (timer < 30 && ! audioplayers[1].activeSelf)
            {
                audioplayers[0].SetActive(false);
                audioplayers[1].SetActive(true);
            }
            TrimText();
            bool go = true;
            foreach (Quest q in todo)
            {
                if(q.Description == ">Escape." && (todo[1].Complete && todo[2].Complete && todo[3].Complete && todo[4].Complete && todo[5].Complete && todo[6].Complete) && wincol && !q.Complete)
                {
                    todo[0].Complete = true;
                    RedoTodo();
                    go = true;
                }
                if (!q.Complete && q.Primary)
                {
                    go = false;
                }
            }
            if (go && !gameoverplaying)
            {
                StartCoroutine(GameOver(false));
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (texts[2].transform.parent.parent.gameObject.activeSelf)
                {
                    texts[2].transform.parent.parent.gameObject.SetActive(false);
                    texts[2].transform.parent.gameObject.SetActive(false);
                    texts[2].gameObject.SetActive(false);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(Resources.Load("Wav/Inventory") as AudioClip, Vector3.zero, 1);
                    texts[2].transform.parent.parent.gameObject.SetActive(true);
                    texts[2].transform.parent.gameObject.SetActive(true);
                    texts[2].gameObject.SetActive(true);
                    WriteInv();
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Reticles[0].SetActive(false);
                Reticles[1].SetActive(true);
                if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out hit, Mathf.Infinity, layerMask))
                {
                    string n = hit.transform.name;
                    var rand = Random.value;
                    switch (hit.transform.tag)
                    {
                        case "Interactable":
                            colstart = "<color=maroon>";
                            if (n.Contains("Painting"))
                            {
                                if (n.Contains("1"))
                                {
                                    AddToSpeach(colstart + "What a lovely desert." + colend);
                                    p[0] = true;
                                }
                                else if (n.Contains("2"))
                                {
                                    StartCoroutine(PaintingTilt(hit.transform.gameObject));
                                    AddToSpeach(colstart + "aaaaah, picturesque mountains." + colend);
                                    p[1] = true;
                                }
                                else if (n.Contains("3"))
                                {
                                    AddToSpeach(colstart + "Fields? its pretty but my hayfever would be horrible." + colend);
                                    p[2] = true;
                                }
                                else if (n.Contains("4"))
                                {
                                    AddToSpeach(colstart + "Orange sky at night, shepards delight... I wonder what that actually means." + colend);
                                    p[3] = true;
                                }
                                else
                                {
                                    AddToSpeach(colstart + "Tasteful." + colend);
                                    p[4] = true;
                                }
                                if (p[0] && p[1] && p[2] && p[3] && p[4])
                                {
                                    todo[7].Complete = true;
                                    RedoTodo();
                                }
                                int temp = 0;
                                foreach (bool b in p)
                                {
                                    if (b)
                                    {
                                        temp++;
                                    }
                                }
                                todo[7].Description = ">Admire Artwork " + temp + "/5";

                            }
                            else if (n.Contains("Switch"))
                            {
                                AudioSource.PlayClipAtPoint(Resources.Load("Wav/Switch") as AudioClip, Vector3.zero, 1);

                                if (hit.transform.rotation.z == 0)
                                {
                                    hit.transform.rotation = Quaternion.Euler(0, -90, 180);
                                }
                                else
                                {
                                    hit.transform.rotation = Quaternion.Euler(0, -90, 0);
                                }
                                if (switches[0].transform.rotation.eulerAngles.z == 180 && switches[1].transform.rotation.eulerAngles.z == 0 && switches[2].transform.rotation.eulerAngles.z == 0 && switches[3].transform.rotation.eulerAngles.z == 180 && switches[4].transform.rotation.eulerAngles.z == 0 && switches[5].transform.rotation.eulerAngles.z == 180)
                                {
                                    todo[4].Complete = true;
                                    RedoTodo();
                                }
                            }
                            else if (n.Contains("Coin"))
                            {
                                AudioSource.PlayClipAtPoint(Resources.Load("Wav/Pickup Gold") as AudioClip, Vector3.zero, 1);
                                int num;
                                var str = n.Substring(n.Length - 1, 1);
                                int.TryParse(str, out num);
                                change += num;
                                Destroy(hit.transform.gameObject);
                                todo[8].Description = ">Find Change" + change.ToString() + "/17p";
                                if (change == 17)
                                {
                                    todo[8].Complete = true;
                                }
                                RedoTodo();
                            }
                            else if (n.Contains("Key"))
                            {
                                AudioSource.PlayClipAtPoint(Resources.Load("Wav/Pickup Key") as AudioClip, Vector3.zero, 1);
                                keys++;
                                AddToInv(hit.transform.gameObject);
                                if (keys == 2)
                                {
                                    todo[5].Complete = true;
                                    RedoTodo();
                                }
                            }
                            else if (n.Contains("Knife") || n.Contains("ToiletPaper") || n.Contains("Spoon") || n.Contains("Rock") || n.Contains("Scissors") || n.Contains("Book"))
                            {
                                AudioSource.PlayClipAtPoint(Resources.Load("Wav/Pickup Obj") as AudioClip, Vector3.zero, 1);

                                AddToInv(hit.transform.gameObject);
                                if (n.Contains("ToiletPaper"))
                                {
                                    todo[2].Complete = true;
                                    RedoTodo();
                                }
                            }
                            else if (n.Contains("Lamp"))
                            {
                                AudioSource.PlayClipAtPoint(Resources.Load("Wav/Switch") as AudioClip, Vector3.zero, 1);

                                if (hit.transform.GetChild(0).gameObject.activeSelf)
                                {
                                    hit.transform.GetChild(0).gameObject.SetActive(false);
                                    if (n.Contains("2"))
                                    {
                                        coderevealed = true;
                                        texts[3].gameObject.SetActive(true);
                                        colstart = "<color=cyan>";
                                        texts[3].text = colstart + "UDDUDU" + colend;
                                        todo[6].Complete = true;
                                        RedoTodo();
                                    }
                                }
                                else
                                {
                                    hit.transform.GetChild(0).gameObject.SetActive(true);
                                    if (n.Contains("2"))
                                    {
                                        texts[3].gameObject.SetActive(false);
                                    }
                                }
                            }
                            else if (n.Contains("Door"))
                            {
                                if ((keys == 1 && (n.Contains("3"))) || keys == 2 && (n.Contains("2")) || (n.Contains("1") && todo[4].Complete) || (n.Contains("4") && washedhands))
                                {
                                    KillDoor(hit);
                                    AudioSource.PlayClipAtPoint(Resources.Load("Wav/Open Door") as AudioClip, Vector3.zero, 1);
                                    todo[3].Description = ">Open Doors " + doors.ToString() + "/4.";
                                    RedoTodo();
                                }
                                else if (n.Contains("3") || n.Contains("2"))
                                {
                                    AddToSpeach(colstart + "This door needs a key." + colend);
                                }
                                else if (n.Contains("1"))
                                {
                                    AddToSpeach(colstart + "This door is activated by those switches." + colend);
                                }
                                else
                                {
                                    AddToSpeach(colstart + "I need to go to the loo and wash my hands before i open that door really." + colend);
                                }
                            }
                            else if (n.Contains("Toilet"))
                            {
                                if (!gonetotoilet)
                                {
                                    AudioSource.PlayClipAtPoint(Resources.Load("Wav/Water") as AudioClip, Vector3.zero, 1);

                                    AddToSpeach(colstart + "Thats better" + colend);
                                    gonetotoilet = true;
                                }
                                else
                                {
                                    AddToSpeach(colstart + "I don't need to go right now." + colend);
                                }
                            }
                            else if (n.Contains("Sink"))
                            {
                                AudioSource.PlayClipAtPoint(Resources.Load("Wav/Water") as AudioClip, Vector3.zero, 1);
                                AddToSpeach(colstart + "Nice and Clean" + colend);
                                if (gonetotoilet)
                                {
                                    washedhands = true;
                                    todo[1].Complete = true;
                                    RedoTodo();
                                }
                            }
                            break;
                        case "Level":
                            colstart = "<color=green>";
                            rand *= 7;
                            if (rand < 7)
                            {
                                AddToSpeach(colstart + "How boring are these blocks." + colend);
                            }
                            else if (rand < 2)
                            {
                                AddToSpeach(colstart + "Each block is made up of six bricks, thats alot of bricks." + colend);
                            }
                            else if (rand < 3)
                            {
                                AddToSpeach(colstart + "Each block can hold 10KN of force, sturdy." + colend);
                            }
                            else if (rand < 4)
                            {
                                AddToSpeach(colstart + "oooh, aggregate blocks." + colend);
                            }
                            else if (rand < 5)
                            {
                                AddToSpeach(colstart + "Grey? How dull." + colend);
                            }
                            else if (rand < 6)
                            {
                                AddToSpeach(colstart + "These look heavy" + colend);
                            }
                            else
                            {
                                AddToSpeach(colstart + "High Density Blocks, interesting choice." + colend);
                            }
                            break;
                        case "Furniture":
                            colstart = "<color=purple>";
                            AddToSpeach(colstart + "Thats a " + n + "." + colend);
                            break;
                        default:
                            //do nothing
                            break;
                    }
                }
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Reticles[0].SetActive(true);
                Reticles[1].SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && Time.timeScale == 0)
        {
            texts[4].transform.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
        else if(Time.timeScale == 1 && timer <= 0)
        {
            StartCoroutine(GameOver(true));
        }
    }
    private IEnumerator GameOver(bool loss)
    {
        if (!gameoverplaying)
        {
            gameoverplaying = true;
            if (loss)
            {
                AudioSource.PlayClipAtPoint(Resources.Load("Wav/Lose") as AudioClip, Vector3.zero, 1);
                gameover.SetActive(true);
            }
            else
            {
                AudioSource.PlayClipAtPoint(Resources.Load("Wav/Win") as AudioClip, Vector3.zero, 1);
            }
            yield return new WaitForSeconds(5);
            gameoverplaying = false;
            SceneManager.LoadScene("Menu");
        }
    }
    private IEnumerator Timer()
    {
        while (timer >= 0)
        {
            texts[5].text = timer.ToString();
            yield return new WaitForSeconds(1);
            timer--;
            yield return null;
        }
    }
    private void WriteInv()
    {
        if (texts[2].transform.parent.parent.gameObject.activeSelf && texts[2].transform.parent.gameObject.activeSelf)
        {
            string tex = string.Empty;
            if (inventory.Count > 0)
            {
                foreach (string s in inventory)
                {
                    tex += s + "\n";
                }
                texts[2].text = tex;
            }
        }
    }
    private void KillDoor(RaycastHit h)
    {
        h.transform.GetComponent<Rigidbody>().isKinematic = false;
        h.transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
        Destroy(h.transform.GetComponent<Collider>());
        Destroy(h.transform.GetChild(0).GetComponent<Collider>());
        doors++;
        if(doors == 4)
        {
            todo[3].Complete = true;
            RedoTodo();
        }
    }
    private void RedoTodo()
    {
        string st = string.Empty;
        foreach (Quest q in todo)
        {
            if (q.Complete)
            {
                st += "<color=green>" + q.Description + colend +"\n";
            }
            else if(q.Primary)
            {
                st += "<color=red>" + q.Description + colend + "\n";
            }
            else
            {
                st += "<color=blue>" + q.Description + colend + "\n";
            }
        }
        texts[0].text = st;
    }
    private void AddToSpeach(string s)
    {
        messages.Add(s + "\n");
        texts[1].text = string.Empty;
        foreach (string t in messages)
        {
            texts[1].text += t;
        }
    }
    private void TrimText()
    {
        if (texts[1].text.Length > 200 && messages.Count > 1)
        {
            messages.RemoveAt(0);
        }
    }
    private IEnumerator PaintingTilt(GameObject painting)
    {
        int i= 0;
        while (i< 100)
        {
            i++;
            painting.transform.RotateAround(painting.transform.position + painting.GetComponent<Collider>().bounds.extents, painting.transform.forward, 0.25f);
            yield return null;
        }
        AddToSpeach(colstart + "Whoops, i don't think that was meant to happen." + colend);
    }
    private void AddToInv(GameObject go)
    {
        colstart = "<color=darkblue>";
        AddToSpeach(colstart + "Yoink!" + colend);
        inventory.Add(go.name);
        Destroy(go);
        if (inventory.Contains("Rock") && inventory.Contains("Book") && inventory.Contains("Scissors"))
        {
            todo[9].Complete = true;
            inventory.Remove("Rock");
            inventory.Remove("Book");
            inventory.Remove("Scissors");
            RedoTodo();
        }
        WriteInv();
    }
    private void RemoveFromInv(string s)
    {
        if (inventory.Contains(s))
        {
            inventory.Remove(s);
            WriteInv();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.name.Contains("EscapeCollider"))
        {
            wincol = true;
        }
    }
}
class Quest
{
    internal string Description;
    internal bool Complete, Primary;
    internal Quest(string desc, bool comp, bool prim)
    {
        Description = desc;
        Complete = comp;
        Primary = prim;
    }
}
