using UnityEngine;

namespace Interactables
{
    public class DestroyOnTouch : MonoBehaviour, IInteractable
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void Interact()
        {
            Destroy(gameObject);
        }
    }
}
