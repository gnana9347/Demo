package com.k7it.service;

import java.io.IOException;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Service;
import org.springframework.web.multipart.MultipartFile;

import com.k7it.dto.VisitorDTO;
import com.k7it.model.Visitor;
import com.k7it.repo.VisitorRepo;

@Service
public class VisitorService {

	private final VisitorRepo visitorRepo;

	@Autowired
	public VisitorService(VisitorRepo visitorRepo) {
		this.visitorRepo = visitorRepo;
	}

	public void saveVisitor(VisitorDTO visitorDTO) throws IOException {

		Visitor visitor = new Visitor();

		visitor.setAge(visitorDTO.getAge());
		visitor.setName(visitorDTO.getName());
		visitor.setEmail(visitorDTO.getEmail());
		visitor.setPhone(visitorDTO.getPhone());
		  MultipartFile aadharFile = visitorDTO.getAadhar();
	        MultipartFile photoFile = visitorDTO.getPhoto();
		  if (aadharFile != null && !aadharFile.isEmpty()) {
			  visitor.setAadhar(aadharFile.getBytes());
			  visitor.setAadharFileName(aadharFile.getOriginalFilename());
	        }
	        if (photoFile != null && !photoFile.isEmpty()) {
	        	visitor.setPhoto(photoFile.getBytes());
	        	visitor.setPhotoFileName(photoFile.getOriginalFilename());
	        }

	        visitorRepo.save(visitor);

	}
    public List<Visitor> getVisitor() {
        return visitorRepo.findAll();
    }

    public Visitor getVisitorById(String id) {
        return visitorRepo.findById(id).orElse(null);
    }

    public ResponseEntity<Visitor> updateVisitor(String id, VisitorDTO visitorDTO) throws IOException {
    	Visitor visitor = visitorRepo.findById(id).orElse(null);
        if (visitor != null) {
        	visitor.setName(visitorDTO.getName());
        	visitor.setEmail(visitorDTO.getEmail());
        	visitor.setPhone(visitorDTO.getPhone());
        	visitor.setAge(visitorDTO.getAge());

            MultipartFile aadharFile = visitorDTO.getAadhar();
            MultipartFile photoFile = visitorDTO.getPhoto();

            if (aadharFile != null && !aadharFile.isEmpty()) {
            	visitor.setAadhar(aadharFile.getBytes());
            	visitor.setAadharFileName(aadharFile.getOriginalFilename());
            }
            if (photoFile != null && !photoFile.isEmpty()) {
            	visitor.setPhoto(photoFile.getBytes());
            	visitor.setPhotoFileName(photoFile.getOriginalFilename());
            }

            visitorRepo.save(visitor);
            return ResponseEntity.ok(visitor);
        } else {
            return ResponseEntity.notFound().build();
        }
    }

    public ResponseEntity<Void> deleteVisitor(String id) {
    	visitorRepo.deleteById(id);
        return ResponseEntity.noContent().build();
    }
	
	
	
	
}
