package com.k7it.wrapperPractice;

import java.io.FileInputStream;
import java.io.FileReader;
import java.io.IOException;
import java.util.Properties;
public class Dynamic_Values_3rdway {

	public static void main(String[] args) throws IOException {
		Properties p=System.getProperties();
		FileReader file=new FileReader("C:\\Users\\Lucky\\eclipse-workspace\\wrapperPractice\\src\\com\\k7it\\wrapperPractice\\Myproperties.properties");
		//FileInputStream in=new FileInputStream("C:\\Users\\Lucky\\eclipse-workspace\\wrapperPractice\\src\\com\\k7it\\wrapperPractice\\Myproperties.properties");
		//p.load(in);
		p.load(file);
/*		String name=System.getProperty("name");
		int age=Integer.parseInt(System.getProperty("age"));
		double ht=Double.parseDouble(System.getProperty("ht"));
		double wt=Double.parseDouble(System.getProperty("wt"));
		System.out.println("name:"+name+" age:"+age+" ht:"+ht+" wt:"+wt);*/
		System.out.println(p.getProperty("name"));
		System.out.println(p.getProperty("age"));
		System.out.println(p.getProperty("ht"));
		System.out.println(p.getProperty("wt"));
	}

}
