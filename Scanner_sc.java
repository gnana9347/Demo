package com.k7it.wrapperPractice;
import java.util.Scanner;

public class Scanner_sc {
	

	public static void main(String[] args) {
		boolean flag=false;
		
		do {
		//providing dynamic values using main method.
		System.out.println(args[0]);
		System.out.println(args[1]);
		System.out.println(args[2]);

		//providing dynamic values using Scanner class.
		Scanner sc=new Scanner(System.in);
		System.out.println("please enter your name");
		String str1=sc.nextLine();
		System.out.println("please enter your age");
		int str2=sc.nextInt();
		System.out.println("please enter your ht");
		double str3=sc.nextDouble();
		System.out.println("please enter your wt");
		double str4=sc.nextDouble();
		System.out.println("please enter your phone");
		long str5=sc.nextLong();
		
		System.out.println("name:"+str1+",age:"+str2+",ht:"+str3+",wt:"+str4+",phone:"+str5);
		
		System.out.println("if you want to continue :true/false");
		 flag=sc.nextBoolean();
		
		}
		while(flag);
		
		System.out.println("execution stoppedd");
		
	}
}
