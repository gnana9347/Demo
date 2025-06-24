package com.k7it.Assignment2;

import java.util.Scanner;

public class pattern3 {
	public static void main(String[] args) {
	
	Scanner sc = new Scanner(System.in);
	System.out.println("enter n value");
	int n = sc.nextInt();
	int m=n,o=n,p=n,q=n,r=n,s=n;
	for (int i=1;i<=n;i++) {
		for(int j=1;j<=n;j++) {
			if(j==1 || i+j==n || j==i-1) {
				System.out.print("*");
			}
			else {
				System.out.print("  ");
			}
		}
		System.out.println("");
	}
	System.out.println("    ");
	/*
	 * System.out.println("enter m value"); int m = sc.nextInt();
	 */
	for(int i=1;i<=m;i++) {
		for(int j=1;j<=m;j++) {
			if(i==1 || /*i+j==n*/ j==n) {
				System.out.print("* ");
			}
			else{
				System.out.print("  ");
			}
			
		}
		System.out.println("   ");
	}
	System.out.println("    ");
	/*
	 * System.out.println("enter o value"); int o = sc.nextInt();
	 */
	for(int i=1;i<=o;i++) {
		for(int j=1;j<=o;j++) {
			if(i==1 ||i==n || j==3) {
				System.out.print("* ");
			}
			else{
				System.out.print("  ");
			}
		}
		System.out.println("   ");
	}
	System.out.println("    ");
	/*
	 * System.out.println("enter p value"); int p = sc.nextInt();
	 */
	for(int i=1;i<=p;i++) {
		for(int j=1;j<=p;j++) {
			if(i==1 || j==3) {
				System.out.print("* ");
			}
			else{
				System.out.print("  ");
			}
		}
		System.out.println("   ");
	}
	System.out.println("    ");
	/*
	 * System.out.println("enter q value"); int q = sc.nextInt();
	 */
	for(int i=1;i<=q;i++) {
		for(int j=1;j<=q;j++) {
			if(i==1 || j==1 || i==n || i==3) {
				System.out.print("* ");
			}
			else{
				System.out.print("  ");
			}
		}
		System.out.println("   ");
	}
	System.out.println("    ");
	/*
	 * System.out.println("enter r value"); int r = sc.nextInt();
	 */
	for(int i=1;i<=r;i++) {
		for(int j=1;j<=r;j++) {
			if(i==1 || j==1 ||i==n) {
				System.out.print("* ");
			}
			else{
				System.out.print("  ");
			}
		}
		System.out.println("   ");
	}
	System.out.println("    ");
	/*
	 * System.out.println("enter s value"); int s = sc.nextInt();
	 */
	for(int i=1;i<=s;i++) {
		for(int j=1;j<=s;j++) {
			if(j==1 || j==n || i==3) {
				System.out.print("* ");
			}
			else{
				System.out.print("  ");
			}
		}
		System.out.println("   ");
	}
	System.out.println("    ");
	}
}
