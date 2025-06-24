package com.k7it.Assignment2;
import static com.k7it.Assignment2.Person.name;  

//import java.beans.DefaultPersistenceDelegate;
public class Person_Manager {
 public static int number=1223;
//private String string;
 
 	public Person_Manager(String string1,String string2) {
 		//this.string=string1;
 		System.out.println(string1+"   "+string2);
 		
}
 	public Person_Manager(String string) {
 		System.out.println(string);
 	}


	public static void Print() {
 	//reading using normal for loop
		/*
		 * for(int i=0;i<a.length;i++) { System.out.println(a[i]); }
		 */
 		
 	//reading using enhanced for loop
		/*
		 * for(int element :a) { System.out.println(element); }
		 */
 	//reading using while loop
		/*
		 * int i=0; while(i<a.length) { System.out.println(a[i]); i++; }
		 */
 	//reading using do while
		int []a= {21,22,23,24,25};
 		int i=0;
 		do {
 			System.out.println(a[i]);
 			i++;
 		}
 		while(i<a.length);
 	}
 	
	
	public static void main(String[] args) {
		System.out.println(Person.name);
		System.out.println(name);
		
		int []b=new int[3];
		b[0]=12;
		System.out.println(b[0]);
		//1way for  derived
		Person_Manager[] p3=new Person_Manager[] {
			new Person_Manager("lucky","s002"),
			new Person_Manager("charvi","s005"),
			new Person_Manager("gagana","s006"),
			
		};
		//2nd way for derived 
		Person_Manager[] p4= {
				new Person_Manager("vinnu", "s003"),
				new Person_Manager("chethan", "s004"),
				new Person_Manager("Bhuvan", "s005")
		};
		//3rd way for derived
		Person_Manager []p5=new Person_Manager[3];
		p5[0]=new Person_Manager("mokshith", "s007");
		p5[1]=new Person_Manager("bhuvi", "s008");
		p5[2]=new Person_Manager("moksha");
		//p5[3]=new Person_Manager("hello");
		
	//System.out.println(p5[0]);
		Print();
	}

}
