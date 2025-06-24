
package com.k7it.controller;

import java.io.IOException;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import com.k7it.dto.VisitorDTO;
import com.k7it.dto.VisitorDTO;
import com.k7it.model.Visitor;
import com.k7it.service.VisitorService;
import com.k7it.service.VisitorService;

@RestController
@RequestMapping("/api/v1")
public class VisitorController {

	@Autowired
	private VisitorService visitorService;

	@PostMapping("/visitor")
	public ResponseEntity<String> createVisitor(@Validated @ModelAttribute VisitorDTO visitorDTO, RedirectAttributes redirectAttributes)
			throws IOException {

		try {
			visitorService.saveVisitor(visitorDTO);
			return ResponseEntity.ok("Visitor Created successfully!");
			
		} catch (IOException e) {
			return ResponseEntity.status(500).body("Failed to Create Visitor: " + e.getMessage());
		}
	}
	
	

    @GetMapping("/visitor/all")
    public List<Visitor> getAllVisitors() {
        return visitorService.getVisitor();
    }

    @GetMapping("/visitor/{id}")
    public Visitor getVisitorById(@PathVariable String id) {
        return visitorService.getVisitorById(id);
    }

    @PutMapping("/visitor/{id}")
    public ResponseEntity<Visitor> updateVisitor(@PathVariable String id, @Validated @ModelAttribute VisitorDTO visitorDTO) throws IOException {
        return visitorService.updateVisitor(id, visitorDTO);
    }

    @DeleteMapping("/visitor/{id}")
    public ResponseEntity<Void> deleteVisitor(@PathVariable String id) {
        return visitorService.deleteVisitor(id);
    }

    @GetMapping("/visitor/aadhar/{id}")
    public ResponseEntity<byte[]> getAadhar(@PathVariable String id) {
    	Visitor visitor = visitorService.getVisitorById(id);
        if (visitor.getAadhar() != null) {
            HttpHeaders headers = new HttpHeaders();
            headers.set(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename="+visitor.getAadharFileName());
            headers.setContentType(MediaType.APPLICATION_OCTET_STREAM);
            return ResponseEntity.ok().headers(headers).body(visitor.getAadhar());
        } else {
            return ResponseEntity.notFound().build();
        }
    }
    
    @GetMapping("/visitor/photo/{id}")
    public ResponseEntity<byte[]> getPhoto(@PathVariable String id) {
    	Visitor visitor = visitorService.getVisitorById(id);
        if (visitor.getPhoto() != null) {
            HttpHeaders headers = new HttpHeaders();
            headers.set(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename="+visitor.getPhotoFileName());
            headers.setContentType(MediaType.APPLICATION_OCTET_STREAM);
            return ResponseEntity.ok().headers(headers).body(visitor.getPhoto());
        } else {
            return ResponseEntity.notFound().build();
        }
    }

    private String getFileExtension(String fileName) {
        if (fileName == null || !fileName.contains(".")) {
            return "";
        }
        return fileName.substring(fileName.lastIndexOf('.') + 1);
    }
}
