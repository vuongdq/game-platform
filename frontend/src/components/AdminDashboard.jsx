import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import {
  Container,
  Paper,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Button,
  Select,
  MenuItem,
  FormControl,
  Box,
  Alert,
  Tabs,
  Tab,
  TextField,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Snackbar,
  CircularProgress
} from '@mui/material';

const AdminDashboard = () => {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState(0);
  const [users, setUsers] = useState([]);
  const [games, setGames] = useState([]);
  const [categories, setCategories] = useState([]);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(true);
  const [openDialog, setOpenDialog] = useState(false);
  const [dialogType, setDialogType] = useState('');
  const [selectedItem, setSelectedItem] = useState(null);
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    imageUrl: '',
    gameUrl: '',
    categoryId: '',
    name: '',
    categoryDescription: ''
  });

  useEffect(() => {
    if (user?.role === 'Admin') {
      fetchData();
    }
  }, [user, activeTab]);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError('');
      const [usersRes, gamesRes, categoriesRes] = await Promise.all([
        fetch('http://localhost:5000/api/admin/users', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        }),
        fetch('http://localhost:5000/api/game', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        }),
        fetch('http://localhost:5000/api/category', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        })
      ]);

      if (!usersRes.ok) {
        const errorData = await usersRes.json();
        throw new Error(errorData.message || 'Failed to fetch users');
      }
      if (!gamesRes.ok) {
        const errorData = await gamesRes.json();
        throw new Error(errorData.message || 'Failed to fetch games');
      }
      if (!categoriesRes.ok) {
        const errorData = await categoriesRes.json();
        throw new Error(errorData.message || 'Failed to fetch categories');
      }

      const [usersData, gamesData, categoriesData] = await Promise.all([
        usersRes.json(),
        gamesRes.json(),
        categoriesRes.json()
      ]);

      setUsers(usersData);
      setGames(gamesData);
      setCategories(categoriesData);
    } catch (err) {
      setError('Failed to load data: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleTabChange = (event, newValue) => {
    setActiveTab(newValue);
  };

  const handleOpenDialog = (type, item = null) => {
    setDialogType(type);
    setSelectedItem(item);
    if (item) {
      setFormData({
        title: item.title || '',
        description: item.description || '',
        imageUrl: item.imageUrl || '',
        gameUrl: item.gameUrl || '',
        categoryId: item.categoryId || '',
        name: item.name || '',
        categoryDescription: item.description || ''
      });
    } else {
      setFormData({
        title: '',
        description: '',
        imageUrl: '',
        gameUrl: '',
        categoryId: '',
        name: '',
        categoryDescription: ''
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setSelectedItem(null);
    setFormData({
      title: '',
      description: '',
      imageUrl: '',
      gameUrl: '',
      categoryId: '',
      name: '',
      categoryDescription: ''
    });
  };

  const handleSubmit = async () => {
    try {
      setError('');
      let url = '';
      let method = '';
      let body = {};

      if (dialogType === 'game') {
        url = selectedItem
          ? `http://localhost:5000/api/game/${selectedItem.id}`
          : 'http://localhost:5000/api/game';
        method = selectedItem ? 'PUT' : 'POST';
        body = {
          title: formData.title,
          description: formData.description,
          imageUrl: formData.imageUrl,
          gameUrl: formData.gameUrl,
          categoryId: parseInt(formData.categoryId)
        };
      } else if (dialogType === 'category') {
        url = selectedItem
          ? `http://localhost:5000/api/category/${selectedItem.id}`
          : 'http://localhost:5000/api/category';
        method = selectedItem ? 'PUT' : 'POST';
        body = {
          name: formData.name,
          description: formData.categoryDescription,
          imageUrl: formData.imageUrl
        };
      }

      const response = await fetch(url, {
        method,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(body)
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Failed to save data');
      }

      setSuccess(`${dialogType} ${selectedItem ? 'updated' : 'created'} successfully`);
      handleCloseDialog();
      fetchData();
    } catch (err) {
      setError('Failed to save data: ' + err.message);
    }
  };

  const handleDelete = async (type, id) => {
    if (window.confirm(`Are you sure you want to delete this ${type}?`)) {
      try {
        setError('');
        const url = type === 'game'
          ? `http://localhost:5000/api/game/${id}`
          : `http://localhost:5000/api/category/${id}`;

        const response = await fetch(url, {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });

        if (!response.ok) {
          const errorData = await response.json();
          if (errorData.games) {
            throw new Error(
              `Cannot delete category because it has ${errorData.count} associated games: ${errorData.games.join(', ')}`
            );
          }
          throw new Error(errorData.message || 'Failed to delete item');
        }

        const data = await response.json();
        setSuccess(data.message || `${type} deleted successfully`);
        fetchData();
      } catch (err) {
        setError('Failed to delete item: ' + err.message);
      }
    }
  };

  const handleRoleChange = async (userId, newRole) => {
    try {
      setError('');
      const response = await fetch(`http://localhost:5000/api/admin/users/${userId}/role`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(newRole)
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Failed to update role');
      }

      const data = await response.json();
      setSuccess(data.message || 'User role updated successfully');
      setUsers(users.map(user => 
        user.id === userId ? { ...user, role: newRole } : user
      ));
    } catch (err) {
      setError('Failed to update user role: ' + err.message);
    }
  };

  const handleDeleteUser = async (userId) => {
    if (window.confirm('Are you sure you want to delete this user?')) {
      try {
        setError('');
        const response = await fetch(`http://localhost:5000/api/admin/users/${userId}`, {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });

        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.message || 'Failed to delete user');
        }

        const data = await response.json();
        setSuccess(data.message || 'User deleted successfully');
        setUsers(users.filter(user => user.id !== userId));
      } catch (err) {
        setError('Failed to delete user: ' + err.message);
      }
    }
  };

  const handleCloseSnackbar = () => {
    setError('');
    setSuccess('');
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="lg">
      <Box sx={{ width: '100%', mt: 4 }}>
        <Tabs value={activeTab} onChange={handleTabChange}>
          <Tab label="Users" />
          <Tab label="Games" />
          <Tab label="Categories" />
        </Tabs>

        {error && (
          <Alert severity="error" sx={{ mt: 2 }}>
            {error}
          </Alert>
        )}

        {success && (
          <Alert severity="success" sx={{ mt: 2 }}>
            {success}
          </Alert>
        )}

        {activeTab === 0 && (
          <TableContainer component={Paper} sx={{ mt: 2 }}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Username</TableCell>
                  <TableCell>Email</TableCell>
                  <TableCell>Role</TableCell>
                  <TableCell>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {users.map((user) => (
                  <TableRow key={user.id}>
                    <TableCell>{user.username}</TableCell>
                    <TableCell>{user.email}</TableCell>
                    <TableCell>
                      <FormControl>
                        <Select
                          value={user.role}
                          onChange={(e) => handleRoleChange(user.id, e.target.value)}
                        >
                          <MenuItem value="User">User</MenuItem>
                          <MenuItem value="Admin">Admin</MenuItem>
                        </Select>
                      </FormControl>
                    </TableCell>
                    <TableCell>
                      <Button
                        color="error"
                        onClick={() => handleDeleteUser(user.id)}
                      >
                        Delete
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}

        {activeTab === 1 && (
          <>
            <Box sx={{ mt: 2, mb: 2 }}>
              <Button
                variant="contained"
                color="primary"
                onClick={() => handleOpenDialog('game')}
              >
                Add Game
              </Button>
            </Box>
            <TableContainer component={Paper}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Title</TableCell>
                    <TableCell>Description</TableCell>
                    <TableCell>Category</TableCell>
                    <TableCell>Actions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {games.map((game) => (
                    <TableRow key={game.id}>
                      <TableCell>{game.title}</TableCell>
                      <TableCell>{game.description}</TableCell>
                      <TableCell>
                        {categories.find(c => c.id === game.categoryId)?.name}
                      </TableCell>
                      <TableCell>
                        <Button
                          color="primary"
                          onClick={() => handleOpenDialog('game', game)}
                          sx={{ mr: 1 }}
                        >
                          Edit
                        </Button>
                        <Button
                          color="error"
                          onClick={() => handleDelete('game', game.id)}
                        >
                          Delete
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </>
        )}

        {activeTab === 2 && (
          <>
            <Box sx={{ mt: 2, mb: 2 }}>
              <Button
                variant="contained"
                color="primary"
                onClick={() => handleOpenDialog('category')}
              >
                Add Category
              </Button>
            </Box>
            <TableContainer component={Paper}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Name</TableCell>
                    <TableCell>Description</TableCell>
                    <TableCell>Actions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {categories.map((category) => (
                    <TableRow key={category.id}>
                      <TableCell>{category.name}</TableCell>
                      <TableCell>{category.description}</TableCell>
                      <TableCell>
                        <Button
                          color="primary"
                          onClick={() => handleOpenDialog('category', category)}
                          sx={{ mr: 1 }}
                        >
                          Edit
                        </Button>
                        <Button
                          color="error"
                          onClick={() => handleDelete('category', category.id)}
                        >
                          Delete
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </>
        )}

        <Dialog open={openDialog} onClose={handleCloseDialog}>
          <DialogTitle>
            {selectedItem ? `Edit ${dialogType}` : `Add ${dialogType}`}
          </DialogTitle>
          <DialogContent>
            {dialogType === 'game' ? (
              <>
                <TextField
                  autoFocus
                  margin="dense"
                  label="Title"
                  fullWidth
                  value={formData.title}
                  onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                />
                <TextField
                  margin="dense"
                  label="Description"
                  fullWidth
                  multiline
                  rows={4}
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                />
                <TextField
                  margin="dense"
                  label="Image URL"
                  fullWidth
                  value={formData.imageUrl}
                  onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
                />
                <TextField
                  margin="dense"
                  label="Game URL"
                  fullWidth
                  value={formData.gameUrl}
                  onChange={(e) => setFormData({ ...formData, gameUrl: e.target.value })}
                />
                <FormControl fullWidth margin="dense">
                  <Select
                    value={formData.categoryId}
                    onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
                  >
                    {categories.map((category) => (
                      <MenuItem key={category.id} value={category.id}>
                        {category.name}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </>
            ) : (
              <>
                <TextField
                  autoFocus
                  margin="dense"
                  label="Name"
                  fullWidth
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                />
                <TextField
                  margin="dense"
                  label="Description"
                  fullWidth
                  multiline
                  rows={4}
                  value={formData.categoryDescription}
                  onChange={(e) => setFormData({ ...formData, categoryDescription: e.target.value })}
                />
                <TextField
                  margin="dense"
                  label="Image URL"
                  fullWidth
                  value={formData.imageUrl}
                  onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
                />
              </>
            )}
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDialog}>Cancel</Button>
            <Button onClick={handleSubmit} color="primary">
              {selectedItem ? 'Update' : 'Create'}
            </Button>
          </DialogActions>
        </Dialog>
      </Box>
    </Container>
  );
};

export default AdminDashboard; 