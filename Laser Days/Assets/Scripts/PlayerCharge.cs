using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharge : MonoBehaviour {



    //public Slider chargeSlider;
    //public Slider predictingSlider;
    public Text chargeValue;
    public int maxCharge = 100;
    public int currentCharge;
    public int potentialCharge;
    public int flipCost = 0;
    RaycastManager rm;
    public Image radialSlider;
    public Image predictiveRadial;

    private float transitionTimePredictive = .15f;
    private float transitionTimeMain = .3f;


    private void Awake()
    {
        rm = GetComponent<RaycastManager>();
        chargeValue.text = maxCharge.ToString();
        currentCharge = maxCharge;
        potentialCharge = maxCharge;
        radialSlider.fillAmount = 1;
        predictiveRadial.fillAmount = 1;
    }

    public void PlayerInteraction() {


        //change slider to new current charege amt
        float start = ((float)currentCharge / (float)maxCharge);
        currentCharge -= (flipCost + rm.SumSelectedObjects());

        float end = ((float)currentCharge / (float)maxCharge);
        StartCoroutine(TransitionStandard(start, end, transitionTimeMain));
        chargeValue.text = currentCharge.ToString();

    }

    public bool CheckPlayerCharge() {
        //compare flip costs to total 
        return (flipCost + rm.SumSelectedObjects() <= currentCharge);
            }

    public void ItemInteraction(GameObject item) {
        ItemProperties itemProps = item.GetComponent<ItemProperties>();
        if(itemProps.objectCharge){
            
            //Check to see if it can be activated
            if (itemProps.value + flipCost + rm.SumSelectedObjects() <= currentCharge)
            {
                //get starting and ending values for image transition. also change actual charge value
                float start = ((float)currentCharge / (float)maxCharge);
                currentCharge -= (itemProps.value + flipCost + rm.SumSelectedObjects());
                float end = ((float)currentCharge / (float)maxCharge);
                StartCoroutine(TransitionStandard(start, end, transitionTimeMain));

                // for text
                chargeValue.text = currentCharge.ToString();      
            }
        }

        if (itemProps.boost)
        {
            //adds charge value:
            currentCharge += itemProps.value;
            //for slider
            radialSlider.fillAmount = ((float)currentCharge / (float)maxCharge);
             

            //for text
            chargeValue.text = currentCharge.ToString();
            UpdatePredictingSlider();
        }
    }

    // Checks whether the object can be flipped
    public bool Check(GameObject item) {
        ItemProperties ip = item.GetComponent<ItemProperties>();
        return (ip.boost) ? false : (ip.value + flipCost + rm.SumSelectedObjects() <= currentCharge);
    }

    public void UpdatePredictingSlider() {
        int heldValue;
        if (GetComponent<MFPP.Modules.PickUpModule>().heldObject)
            heldValue = GetComponent<MFPP.Modules.PickUpModule>().heldObject.GetComponent<ItemProperties>().value;
        else
            heldValue = 0;

        //get values for predictive slider's transition animation
        float start = predictiveRadial.fillAmount;
        float end = (float)(currentCharge - (flipCost + heldValue + rm.SumSelectedObjects())) / (float)maxCharge;

        StartCoroutine(TransitionPredictive(start, end, transitionTimePredictive));
    }

    //predictive slider animation coroutine
    private IEnumerator TransitionPredictive(float start, float end, float time)
    {
        if (Mathf.Abs(start - end) < .01) { start = end; } //avoiding any math rounding issues (all charge values should be greater than 1%, or .01 on slider

        float elapsedTime = 0;
        float ratio = elapsedTime / time;

        while (ratio < 1f)
            {
                elapsedTime += Time.deltaTime;
                ratio = elapsedTime / time;
                predictiveRadial.fillAmount = Mathf.SmoothStep(start, end, ratio);
                yield return null;
            }
    }

    //main slider animation
    private IEnumerator TransitionStandard(float start, float end, float time)
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / time;
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / time;
            radialSlider.fillAmount = Mathf.SmoothStep(start, end, ratio);
            yield return null;
        }
    }

}

