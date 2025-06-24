package com.k7it.model;

import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

import jakarta.validation.constraints.NotNull;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Document(collection = "Visitors")
@AllArgsConstructor
@NoArgsConstructor
@ToString
public class Visitor {

	@Id
	private String id;

	private String name;

	private String email;

	private String phone;

	private Integer age;

	private byte[] aadhar;  
	private String aadharFileName; // Field to store the original file name of the aadhar
    private byte[] photo;
    private String photoFileName;  // Field to store the original file name of the photo
	public String getId() {
		return id;
	}
	public void setId(String id) {
		this.id = id;
	}
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public String getEmail() {
		return email;
	}
	public void setEmail(String email) {
		this.email = email;
	}
	public String getPhone() {
		return phone;
	}
	public void setPhone(String phone) {
		this.phone = phone;
	}
	public Integer getAge() {
		return age;
	}
	public void setAge(Integer age) {
		this.age = age;
	}
	public byte[] getAadhar() {
		return aadhar;
	}
	public void setAadhar(byte[] aadhar) {
		this.aadhar = aadhar;
	}
	public String getAadharFileName() {
		return aadharFileName;
	}
	public void setAadharFileName(String aadharFileName) {
		this.aadharFileName = aadharFileName;
	}
	public byte[] getPhoto() {
		return photo;
	}
	public void setPhoto(byte[] photo) {
		this.photo = photo;
	}
	public String getPhotoFileName() {
		return photoFileName;
	}
	public void setPhotoFileName(String photoFileName) {
		this.photoFileName = photoFileName;
	}
    
    
}
