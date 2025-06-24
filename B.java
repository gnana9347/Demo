package com.k7it;

class Bc {
	int i = 30;
	String name = "k7it";

	Bc(int i, String name) {
		System.out.println(name);
		System.out.println(i);
	}

	Bc() {
		System.out.println("no_arg");
	}
}

public class B extends Bc {
	public int i = 50;

	public static void main(String[] args) {
		Bc b1 = new Bc(40, "py");

		System.out.println(b1.name);
		B b2 = new B();
		// new B().m1();
		// new B().m2();
		b2.m2();
	}

	public int m1() {

		return 10 + 2;
	}

	void m2() {
		System.out.println(m1());
	}
}
