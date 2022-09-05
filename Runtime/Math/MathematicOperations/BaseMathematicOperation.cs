using Sirenix.OdinInspector;
using UnityEngine;

namespace ChainBehaviors.Math
{
    /// <summary>
    /// Make simple calculus (+, -, *, /, %) between 2 values. 
    /// Values can be dynamics, constants or both (most of the time).
    /// </summary>
    /// <typeparam name="TOperand"></typeparam>
    public abstract class BaseMathematicOperation<TOperand> : MonoBehaviour
    {
        public enum EOperator
        {
            Add,
            Minus,
            Multiply,
            Divide,
            Modulo
        }

        [SerializeField]
        protected EOperator _operator = EOperator.Add;
        public EOperator Operator
        {
            get => _operator;
            set => _operator = value;
        }

        [SerializeField]
        protected TOperand _operandA = default;
        public TOperand OperandA
        {
            get => _operandA;
            set => _operandA = value;
        }

        [SerializeField]
        protected TOperand _operandB = default;
        public TOperand OperandB
        {
            get => _operandB;
            set => _operandB = value;
        }

        [Button("Execute Operation")]
        public abstract void ExecuteOperation();

        public void ExecuteWithOperandA(TOperand operandValue)
        {
            OperandA = operandValue;
            ExecuteOperation();
        }

        public void ExecuteWithOperandB(TOperand operandValue)
        {
            OperandB = operandValue;
            ExecuteOperation();
        }
    }
}