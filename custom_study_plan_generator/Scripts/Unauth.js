document.getElementById('studentID').addEventListener('submit', function (e) { Unauthorise(e); });




function Unauthorise(e) {

    e.preventDefault();

    window.alert('Please create or save a course to upload!');

}