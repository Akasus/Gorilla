using UnityEngine;


//This is a very simplified Version for the Use of a ProgressBar.
//In this case there is no Rectangle that changes its size, its more like a Slider
//that change its position.

namespace UI
{
    public class ValueBar : MonoBehaviour
    {
        private float _max = 100; // Maximum Value

        private float _min; // Minimum Value 

        [SerializeField] private GameObject slider;

        private float _val; // Current Value

        public float Minimum
        {
            get => _min;

            set
            {
                // Prevent a negative Value.
                if (value < 0) _min = 0;

                // Make sure that the minimum Value is never set higher than the Maximum Value.
                if (value > _max)
                {
                    _min = value;
                    _min = value;
                }

                // Ensure Value is still in range
                if (_val < _min) _val = _min;
            }
        }

        public float Maximum
        {
            get => _max;

            set
            {
                // Make sure that the Maximum Value is never set lower than the minimum Value.
                if (value < _min) _min = value;

                _max = value;

                // Make sure that Value is still in range.
                if (_val > _max) _val = _max;
            }
        }

        public float Value
        {
            get => _val;

            set
            {
                var oldValue = _val;
                // Make sure that the Value does not stray outside the valid range.
                if (value < _min)
                    _val = _min;
                else if (value > _max)
                    _val = _max;
                else
                    _val = value;


                var percent = (_val - _min) / (_max - _min);

                SetSliderPosition(percent);
            }
        }

        private void SetSliderPosition(float value)
        {
            slider.transform.localPosition = new Vector2(Mathf.Lerp(-4, 4, value),0);
        }
    }
}