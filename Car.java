
package com.k7it;

interface Vehicle {

	public abstract void moving();

	public abstract void working();

	public static void running() {
		System.out.println("static Method");
	}

	public default void sound() {
		System.out.println("Default Method");
	}
}

class Car implements Vehicle {
	public static void main(String args[]) {
		Car c1 = new Car();
		c1.moving();
		c1.working();
		Vehicle.running();
		c1.running();
		c1.sound();
		Bus c2 = new Bus();

		c2.moving();
		c2.working();
		Vehicle.running();
		c2.sound();
	}

	public void moving() {
		System.out.println("car is moving");
	}

	public void working() {
		System.out.println("car is working");
	}

	public  void running() {
		System.out.println("overriding Method");
	}

}

class Bus implements Vehicle {
	public void moving() {
		System.out.println("Bus is moving");
	}

	public void working() {
		System.out.println("Bus is working");
	}

}
