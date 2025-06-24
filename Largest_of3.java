package com.k7it.Assignment2;

import java.util.Scanner;

public class Largest_of3 {
	public static void main(String[] args) {
		Scanner sc = new Scanner(System.in);
		System.out.println("enter a value");
		int a= sc.nextInt();
		System.out.println("enter b value");
		int b= sc.nextInt();
		System.out.println("enter c value");
		int c= sc.nextInt();
		if(a>b && a>c) {
			System.out.println("a is largest number"+a);
		}
		else if(b>a && b>c) {
			System.out.println("b is largest number"+b);
		}
		else if(c>a && c>b) {
			System.out.println("c is largest number"+c);
		}
		else {
			System.out.println("plz enter valid numbers");
		}
		
	}
}
