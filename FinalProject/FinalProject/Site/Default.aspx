<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FinalProject.Default" %>
<script src="Scripts/Slideshow.js" lang="javascript" type="text/javascript"></script>
<!DOCTYPE html>
 
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="CSS/Default.css" rel="stylesheet" type="text/css" />
   
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:DropDownList ID="Brands" runat="server" OnSelectedIndexChanged="Brands_SelectedIndexChanged"></asp:DropDownList>
            <asp:DropDownList ID="Categories" runat="server" OnSelectedIndexChanged="Categories_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
        </div>
    </form>
  
        
        <!-- Slideshow container -->
<div class="slideshow-container">

  <!-- Full-width images with number and caption text -->
  <div class="mySlides fade">
    <div class="1">1 / 3</div>
    <img src="Photos/2017 Silverardo.jpg" style="width:100%"/>
    <div class="text">Caption Text</div>
  </div>

  <div class="mySlides fade">
    <div class="2">2 / 3</div>
    <img src="Photos/2018 Calarado Preformance.jpg" style="width:100%"/>
    <div class="text">Caption Two</div>
  </div>

  <div class="mySlides fade">
    <div class="3">3 / 3</div>
    <img src="Photos/Preformance Home Page.jpg" style="width:100%"/>
    <div class="text">Caption Three</div>
  </div>

  <!-- Next and previous buttons -->
  <a class="prev" onclick="plusSlides(-1)">&#10094;</a>
  <a class="next" onclick="plusSlides(1)">&#10095;</a>
</div>
<br/>

<!-- The dots/circles -->
<div style="text-align:center">
  <span class="dot" onclick="currentSlide(1)"></span> 
  <span class="dot" onclick="currentSlide(2)"></span> 
  <span class="dot" onclick="currentSlide(3)"></span> 
</div>
  
</body>
</html>
