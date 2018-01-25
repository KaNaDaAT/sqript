﻿namespace Qrakhen.Sqript
{
    internal class Reference : Value<Value>
    {        
        public Reference(Value value) : base(ValueType.REFERENCE, value) {

        }

        public Reference() : base(ValueType.REFERENCE, new Value(null, ValueType.NULL)) {

        }

        public virtual void assign(Value value) { 
            setReference(value);
        }

        public virtual Value getReference() {
            //return value; temporary bypass to see if this method actually serves any purpose at all
            return getTrueValue();
        }

        public virtual Value getTrueValue() {
            return (value is Reference ? (value as Reference).getTrueValue() : value);
        }

        public virtual void setReference(Value value) {
            setValue(value, type);
        }

        public new virtual T getValue<T>() {
            return value.getValue<T>();
        }

        public new virtual object getValue() {
            return getTrueValue()?.getValue();
        }

        public ValueType getValueType() {
            return getTrueValue().type;
        }

        public override string ToString() {
            return (getReference() == null ? NULL.ToString() : getReference().ToString());
        }
    }

    internal class TypeReference : Reference
    {
        public ValueType forcedType {
            get {
                return forcedType;
            }
            private set {
                if (forcedType == ValueType.NULL) forcedType = value;
                else throw new Exception("can not redefine static reference type");
            }
        }

        public TypeReference(Value value, ValueType forcedType) : base(value) {
            this.forcedType = forcedType;
        }
    }

    internal class FloatingReference : Reference
    {
        public string name { get; private set; }
        public Qontext owner { get; private set; }

        public FloatingReference(string name, Qontext owner) : base(NULL) {
            this.name = name;
            this.owner = owner;
        }

        public void bind() {
            if (value.type != ValueType.NULL) owner.set(name, new Reference(value));
        }
    }
}
