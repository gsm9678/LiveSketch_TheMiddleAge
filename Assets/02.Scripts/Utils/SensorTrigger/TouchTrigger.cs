using UnityEngine;

namespace X_Running.Base
{
    abstract public class TouchTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Character")
            {
                TriggerEnter();
            }
        }
        abstract public void TriggerEnter();
    }
}
