﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Reference
    {
        public Value value { get; protected set; }
        public string name { get; protected set; }

        public Reference(string name, Value value = null) {
            this.name = name;
            this.value = (value == null ? Value.NULL : value);
        }

        public virtual void assign(Value value, bool reference = false) {
            if (reference) this.value = value;
            else {
                if (value == Value.NULL) value = new Value(value.type, value.getValue());
                else this.value.setValue(value.getValue(), value.type);
            }
        }

        public ValueType getValueType() {
            return value.type;
        }

        public virtual Value getReference() {
            return value;
        }

        public virtual object getValue() {
            return value.getValue();
        }

        public virtual T getValue<T>() {
            return value.getValue<T>();
        }

        public override string ToString() {
            return getReference().ToString();
        }

        public virtual string toDebug() {
            return name + ": " + getReference().toDebug();
        }

        public class MemberSelect
        {
            public Reference reference;
            public object[] select;

            public MemberSelect(Reference reference, object[] select) {
                this.reference = reference;
                this.select = select;
            }

            public Value getMember(bool parent = false) {
                if (reference.getReference().isType((int)ValueType.ARRAY)) {
                    int[] __keys = new int[select.Length - (parent ? 1 : 0)];
                    for (int i = 0; i < __keys.Length; i++) __keys[i] = (int)select[i];
                    return reference.getValue<Array>().get(__keys);
                } else if (reference.getReference().isType((int)ValueType.OBQECT)) {
                    string[] __keys = new string[select.Length - (parent ? 1 : 0)];
                    for (int i = 0; i < __keys.Length; i++) __keys[i] = (string)select[i];
                    return reference.getValue<Obqect>().get(__keys);
                } else throw new Exception("can not get member of non-collection");
            }

            public void assign(Value value, bool reference = false) {
                if (!reference) getMember().setValue(value.getValue(), value.type);
                else {
                    object parent = getMember(true);
                    if ((parent as Value).type == ValueType.ARRAY) (parent as Array).set((int)select[select.Length - 1], value);
                    else if((parent as Value).type == ValueType.OBQECT)(parent as Obqect).set((string)select[select.Length - 1], value);
                    else throw new Exception("can not set member of non-collection");
                }
            }
        }
    }    
}