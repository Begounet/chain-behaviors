using System;
using UnityEngine;
using AUE;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Math
{
    [AddComponentMenu(CBConstants.ModuleMath + "Math Operation (float)")]
    public class MathFloatOperation : BaseMathematicOperation<float>
    {
        public enum ERoundMode
        {
            None,
            Round,
            Floor,
            Ceil,
            RoundToInt,
            FloorToInt,
            CeilToInt,
        }

        [SerializeField]
        private ERoundMode _roundMode = ERoundMode.None;
        public ERoundMode RoundMode 
        {
            get => _roundMode;
            set => _roundMode = value;
        }

        [SerializeField]
        private AUEEvent<float> _onOperationCompleted;

        public override void ExecuteOperation()
        {
            float result;

            switch (_operator)
            {
                case EOperator.Add:
                    result = _operandA + _operandB;
                    break;
                case EOperator.Minus:
                    result = _operandA - _operandB;
                    break;
                case EOperator.Multiply:
                    result = _operandA * _operandB;
                    break;
                case EOperator.Divide:
                    result = _operandA / _operandB;
                    break;
                case EOperator.Modulo:
                    result = _operandA % _operandB;
                    break;

                default:
                    throw new Exception($"Unsupported operator {_operator}");
            }

            ApplyRound(ref result);
            _onOperationCompleted.Invoke(result);
        }

        private void ApplyRound(ref float result)
        {
            switch (_roundMode)
            {
                case ERoundMode.Round:
                    result = Mathf.Round(result);
                    break;
                case ERoundMode.Floor:
                    result = Mathf.Floor(result);
                    break;
                case ERoundMode.Ceil:
                    result = Mathf.Ceil(result);
                    break;
                case ERoundMode.RoundToInt:
                    result = Mathf.RoundToInt(result);
                    break;
                case ERoundMode.FloorToInt:
                    result = Mathf.FloorToInt(result);
                    break;
                case ERoundMode.CeilToInt:
                    result = Mathf.CeilToInt(result);
                    break;

                default:
                case ERoundMode.None:
                    break;
            }
        }
    }
}