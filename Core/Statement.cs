﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    public class Statement : Interpretoken
    {
        public Statement(Token[] stack) : base(stack) { }

        public void execute(Funqtion context) {
            Debug.spam("executing statement:\n" + ToString());
            Reference target = null;
            Reference.MemberSelect select = null;
            Value result = null;
            do {
                Token t = peek();
                if (t.type == ValueType.KEYWORD) {
                    Keyword keyword = digest().getValue<Keyword>();
                    string identifier = peek().getValue<string>();
                    switch (keyword.name) {
                        case Keyword.DECLARE: target = declareReference(context, digest().getValue<string>()); break;
                        //case Keyword.FUNCTION: target = declareReference(digest().getValue<string>()); break;
                        default: throw new Exception("only reference creation implemented yet", peek());
                    }
                } else if (t.type == ValueType.IDENTIFIER) {
                    target = getReference(context, digest().getValue<string>());
                    if (peek().type == ValueType.STRUCTURE && peek().getValue<string>() == Structure.MEMBER_KEY_DELIMITER) {
                        digest();
                        List<object> buffer = new List<object>();
                        do {
                            if (peek().type == ValueType.IDENTIFIER && target.getValueType() == ValueType.OBQECT) {
                                string key = digest().getValue<string>(); 
                                Reference r = (target.getReference() == null ? null : (target.getReference() as Obqect).get(key));
                                if (r == null) {
                                    r = new Reference(null);
                                    (target.getReference() as Obqect).set(key, r);
                                }
                                target = r;
                            } else if (peek().isType((int)ValueType.NUMBER) && target.getValueType() == ValueType.ARRAY) {
                                int key = digest().getValue<int>();
                                Reference r = (target.getReference() == null ? null : (target.getReference() as Array).get(key));
                                if (r == null) {
                                    r = new Reference(null);
                                    (target.getReference() as Array).set(key, r);
                                }
                                target = r;
                            } else {
                                throw new Exception("unexpected token when trying to resolve member tree", peek());
                            }
                            if (peek().type == ValueType.STRUCTURE && peek().getValue<string>() == Structure.MEMBER_KEY_DELIMITER) {
                                digest();
                                continue;
                            } else break;
                        } while (!endOfStack());
                        //select = new Reference.MemberSelect(target, buffer.ToArray()); 
                    }
                } else if (t.type == ValueType.OPERATOR) {
                    Operator op = digest().getValue<Operator>();
                    Token[] right = new Token[(stack.Length - position)];
                    for (int i = 0; i < right.Length; i++) right[i] = digest();

                    Expression expr;
                    if (right.Length == 1) expr = new Expression(op, target, digest(), context);
                    else expr = new Expression(op, target, new Expressionizer(right).parse(context), context);
                    result = expr.execute();
                } else Debug.warn("unexpected token: '" + digest() + "'");
            } while (!endOfStack());
            if (result != null) Debug.log(result.ToString(), ConsoleColor.Green);
            else if (target != null) Debug.log(target.ToString(), ConsoleColor.Green);
        }

        private Reference declareReference(Context context, string name) {
            Reference r = new Reference();
            context.set(name, r);
            Debug.spam("reference '" + name + "' declared!");
            return r;
        }

        private Reference getReference(Context context, string name) {
            Reference r = context.lookup(name);
            if (r == null) throw new NullReferenceException("unknown reference name '" + name + "' given");
            return r;
        }

        public override string ToString() {
            string str = "";
            foreach (Token token in stack) {
                str += token.getValue().ToString() + " ";
            } 
            return str.Substring(0, str.Length - 1) + ";";
        }
    }
}