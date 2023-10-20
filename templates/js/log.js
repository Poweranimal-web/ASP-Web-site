let url = "http://localhost:5202/login"
const element = document.getElementById("form");
const div = document.getElementById("block");
element.addEventListener("submit", validate);
async function validate(event){
    event.preventDefault();
        const response = await fetch(
            url,
            {
                method: "POST",
                headers: {
                    "Accept": "application/json", 
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    "email": String(document.getElementById("email").value),
                    "password": String(document.getElementById("password").value),
                })
            });
            if (!response.ok){
                throw new Error(`Error! status ${response.status}`);
            }
            else{
                let data = await response.json();
                data = JSON.parse(data);
                if (data.Status=="good"){
                    document.location.replace("/");
                }    
            }
    
    
       
    
}