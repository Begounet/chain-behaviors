#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using AUE;
using System;
using UnityEngine;
using ChainBehaviors.Utils;

namespace ChainBehaviors
{
    /// <summary>
    /// Execute a numeric (float) condition and trigger an event with its result.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleControlsPath + "Numeric Condition")]
    public class NumericCondition : BaseMethod
    {
        public enum Operation
        {
            Equals,
            Less,
            LessOrEquals,
            Greater,
            GreaterOrEquals
        }

        [SerializeField]
        private float _operandA;
        public float OperandA 
        {
            get => _operandA;
            set
            {
                _operandA = value;
                SetInvokerDirty();
            }
        }

        [SerializeField]
        private Operation _operator;
        public Operation Operator
        {
            get => _operator;
            set
            {
                _operator = value;
                SetInvokerDirty();
            }
        }

        [SerializeField]
        private float _operandB;
        public float OperandB
        {
            get => _operandB;
            set
            {
                _operandB = value;
                SetInvokerDirty();
            }
        }

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
        public string Preview => $"{_operandA} {GetPreviewOperatorSymbol(_operator)} {_operandB} = {GetOperationResult()}";
#endif

        [SerializeField, Tooltip("The argument is the result of the condition evaluation")]
        private AUEEvent<bool> _onConditionExecuted;


        private Func<float, float, bool> _operationInvoker;
        private bool _isInvokerDirty = true;


        private void OnValidate()
        {
            SetInvokerDirty();
        }

        public bool SetOperandAThenExecute(int operandA) => SetOperandAThenExecute((float)operandA);
        public bool SetOperandAThenExecute(float operandA)
        {
            OperandA = operandA;
            return Execute();
        }

        public bool SetOperandBThenExecute(int operandB) => SetOperandAThenExecute((float)operandB);
        public bool SetOperandBThenExecute(float operandB)
        {
            OperandB = operandB;
            return Execute();
        }

        public bool SetOperatorThenExecute(Operation operation)
        {
            Operator = operation;
            return Execute();
        }

        public bool Execute()
        {
            bool result = GetOperationResult();
            Trace(("Operand A", _operandA), ("Operator", _operator), ("Operand B", _operandB), ("Result", result));
            _onConditionExecuted.Invoke(result);
            return result;
        }

        private string GetPreviewOperatorSymbol(Operation @operator)
        {
            switch (@operator)
            {
                case Operation.Equals:
                    return ("==");
                case Operation.Less:
                    return ("<");
                case Operation.LessOrEquals:
                    return ("<=");
                case Operation.Greater:
                    return (">");
                case Operation.GreaterOrEquals:
                    return (">=");
                default:
                    return ("<unknown>");
            }
        }

        public bool GetOperationResult()
        {
            CacheOperationInvokerIFN();
            return _operationInvoker?.Invoke(_operandA, _operandB) ?? false;           
        }

        private void CacheOperationInvokerIFN()
        {
            if (!_isInvokerDirty)
            {
                return;
            }

            _operationInvoker = GetOperationInvoker(_operator);

            _isInvokerDirty = false;
        }

        private static Func<float, float, bool> GetOperationInvoker(Operation operation)
        {
            switch (operation)
            {
                case Operation.Equals:
                    return (a, b) => Mathf.Approximately(a, b);
                case Operation.Less:
                    return (a, b) => (a < b);
                case Operation.LessOrEquals:
                    return (a, b) => (a < b || Mathf.Approximately(a, b));
                case Operation.Greater:
                    return (a, b) => (a > b);
                case Operation.GreaterOrEquals:
                    return (a, b) => (a > b || Mathf.Approximately(a, b));

                default:
                    return null;
            }
        }

        public static bool IsTrue(float operandA, Operation @operator, float operandB)
        {
            var operation = GetOperationInvoker(@operator);
            return (operation?.Invoke(operandA, operandB) ?? false);
        }

        private void SetInvokerDirty() => _isInvokerDirty = true;
    }
}
