function togglePassword() {
	const passwordInput = document.getElementById('inputPassword');
	const eyeIcon = document.getElementById('eyeIcon');

	if (passwordInput.type === 'password') {
		passwordInput.type = 'text';
		eyeIcon.classList.remove('fa-eye-slash');
		eyeIcon.classList.add('fa-eye');
	} else {
		passwordInput.type = 'password';
		eyeIcon.classList.remove('fa-eye');
		eyeIcon.classList.add('fa-eye-slash');
	}
}