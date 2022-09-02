using AUE;
using System;
using UnityEngine;

namespace ChainBehaviors.Math
{
    public class MathIntOperation : BaseMathematicOperation<int>
    {
        [SerializeField]
        private AUEEvent<int> _onOperationCompleted;

        public override void ExecuteOperation()
        {
            int result;

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

            _onOperationCompleted.Invoke(result);
        }
    }
}