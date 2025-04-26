import React from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Paper,
  Typography,
  Box,
  Button,
  Grid,
  Card,
  CardContent,
  CardActions
} from '@mui/material';

const Dashboard = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <Container maxWidth="lg">
      <Box sx={{ mt: 4 }}>
        <Paper elevation={3} sx={{ p: 4 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 4 }}>
            <Typography variant="h4" component="h1">
              Welcome, {user?.username}!
            </Typography>
            <Button
              variant="contained"
              color="primary"
              onClick={handleLogout}
            >
              Logout
            </Button>
          </Box>

          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <Card>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    Your Profile
                  </Typography>
                  <Typography variant="body1">
                    Username: {user?.username}
                  </Typography>
                  <Typography variant="body1">
                    Email: {user?.email}
                  </Typography>
                  <Typography variant="body1">
                    Role: {user?.role}
                  </Typography>
                </CardContent>
                <CardActions>
                  <Button size="small" color="primary">
                    Edit Profile
                  </Button>
                </CardActions>
              </Card>
            </Grid>

            {user?.role === 'Admin' && (
              <Grid item xs={12} md={6}>
                <Card>
                  <CardContent>
                    <Typography variant="h6" gutterBottom>
                      Admin Panel
                    </Typography>
                    <Typography variant="body1">
                      Access the admin dashboard to manage users and settings.
                    </Typography>
                  </CardContent>
                  <CardActions>
                    <Button
                      size="small"
                      color="primary"
                      onClick={() => navigate('/admin')}
                    >
                      Go to Admin Panel
                    </Button>
                  </CardActions>
                </Card>
              </Grid>
            )}
          </Grid>
        </Paper>
      </Box>
    </Container>
  );
};

export default Dashboard; 