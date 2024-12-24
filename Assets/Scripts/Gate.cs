using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{
    IEnumerator FlashLight(Color color){
        gameObject.GetComponent<MeshRenderer>().material.color = color;
        hit_hud.color = color - new Color(0,0,0,0.5f);
        yield return new WaitForSeconds(1);
        gameObject.GetComponent<MeshRenderer>().material.color = original_color;
        hit_hud.color = new Color(0,0,0,0);
    }
    private Image hit_hud;
    private Color original_color;

    void Start(){
        hit_hud = GameObject.Find("Hit Indicator").GetComponent<Image>();
        original_color = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    void OnTriggerEnter(Collider collider)
    { 
        Vector3 point = transform.InverseTransformPoint(collider.ClosestPoint(transform.position));
        if (point.x > 0.2 && point.x < 0.8 && point.y > 0.2 && point.y < 0.8){
            
            IEnumerator flash = FlashLight(new Color(0.2f,1,0.2f));
            StartCoroutine(flash);
        } else {
            IEnumerator flash = FlashLight(new Color(1,0.2f,0.2f));
            StartCoroutine(flash);
        }
        
    }
}
